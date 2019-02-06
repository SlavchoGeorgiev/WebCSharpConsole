require(['vs/editor/editor.main'], function () {
    var CONSOLE_CODE_CLASS = 'console-code'

    function ConsoleEmulator(container) {
        var self = this;
        var $container = $(container);
        var $consoleCodeWrapper = $container.find(`.${CONSOLE_CODE_CLASS}`);
        var consoleCodeWrapper = $consoleCodeWrapper[0];
        var $consoleOutput = $container.find('.console-output');
        var consoleOutput = $consoleOutput[0];

        self._consoleOutputEditor = self._initConsoleOutput(consoleOutput);
        self._consoleEditor = self._initConsoleEditor(consoleCodeWrapper);
        self._consoleEditorAddActions();
        self._consoleEditorAddOnDidChangeModelContent();

        self._$runButton = $container.find('.run-button');
        self._$runButton.on('click', function () {
            self._compileAbdRun();
        });
    }

    ConsoleEmulator.prototype = Object.create({});
    ConsoleEmulator.prototype.constructor = ConsoleEmulator;

    ConsoleEmulator.prototype._initConsoleEditor = function (consoleCodeWrapper) {
        var editor = monaco.editor.create(consoleCodeWrapper, {
            value: [
                'using System;',
                'using System.Collections.Generic;',
                'using System.Linq;',
                '',
                'public class Program',
                '{',
                '   public static void Main()',
                '   {',
                '   }',
                '}'
            ].join('\n'),
            language: 'csharp',
            automaticLayout: true,
            lineNumbers: true,
            roundedSelection: true,
            scrollBeyondLastLine: false,
            readOnly: false,
            theme: 'vs-dark',
            wordWrap: 'on',
            wordWrapMinified: true,
            //wordWrap: 'wordWrapColumn',
            //wrappingColumn: 150,
            // try "same", "indent" or "none"
            wrappingIndent: 'indent'
        });

        return editor;
    };

    ConsoleEmulator.prototype._initConsoleOutput = function (consoleOutput) {
        return monaco.editor.create(consoleOutput, {
            value: 'Console output',
            language: 'text',
            automaticLayout: true,
            lineNumbers: true,
            roundedSelection: true,
            scrollBeyondLastLine: false,
            readOnly: false,
            theme: 'vs-dark',
            wordWrap: 'on',
            wordWrapMinified: true,
            wrappingIndent: 'indent'
        });
    };

    ConsoleEmulator.prototype._consoleEditorAddActions = function () {
        var self = this;

        self._consoleEditor.addAction({
            // An unique identifier of the contributed action.
            id: 'compile-and-run-id',
            // A label of the action that will be presented to the user.
            label: 'Run',
            // An optional array of keybindings for the action.
            keybindings: [monaco.KeyMod.CtrlCmd | monaco.KeyCode.F5],
            // A precondition for this action.
            precondition: null,
            // A rule to evaluate on top of the precondition in order to dispatch the keybindings.
            keybindingContext: null,
            contextMenuGroupId: 'navigation',
            contextMenuOrder: 0.5,
            // Method that will be executed when the action is triggered.
            // @param editor The editor instance is passed in as a convinience
            run: function (ed) {
                self._compileAbdRun();
            }
        });
    };

    ConsoleEmulator.prototype._consoleEditorAddOnDidChangeModelContent = function () {
        var self = this;
        var oldDecorations = [];
        var decorationTimeoutId = null;
        self._consoleEditor.onDidChangeModelContent(function (e) {
            clearTimeout(decorationTimeoutId);

            decorationTimeoutId = setTimeout(function () {
                var code = self._consoleEditor.getValue();

                $.ajax({
                    method: "POST",
                    url: "/home/TestCompile",
                    data: { code: code }
                }).done(function (compileResult) {
                    if (compileResult.success) {
                        oldDecorations = self._consoleEditor.deltaDecorations(oldDecorations, []);
                        return;
                    };

                    var newDecorations = compileResult.diagnostics.map(function (diagnostic) {
                        return {
                            range: new monaco.Range(diagnostic.range.startLineNumber,
                                diagnostic.range.startColumnNumber,
                                diagnostic.range.endLineNumber,
                                diagnostic.range.endColumnNumber),
                            options: {
                                inlineClassName: self._getDecorationClassByDiagnostic(diagnostic),
                                hoverMessage: diagnostic.message,
                            }
                        };
                    });

                    oldDecorations = self._consoleEditor.deltaDecorations(oldDecorations, newDecorations);
                });
            }, 2000);
        });
    };

    ConsoleEmulator.prototype._getDecorationClassByDiagnostic = function (diagnostic) {
        switch (diagnostic.diagnosticKind) {
            case 1: return 'code-info'; // Information that does not indicate a problem (i.e.not prescriptive).
            case 2: return 'code-warning'; // Something suspicious but allowed.
            case 3: return 'code-error'; // Something not allowed by the rules of the language or other authority.
            default: return '';
        }
    };

    ConsoleEmulator.prototype._compileAbdRun = function () {
        var self = this;
        var code = self._consoleEditor.getValue();

        self._$runButton.off();
        self._$runButton.button('loading');

        $.ajax({
            method: "POST",
            url: "/home/CompileAndRun",
            data: { code: code }
        }).done(function (runResult) {
            var newConsoleOutput;
            if (runResult.success) {
                newConsoleOutput = `${runResult.consoleOutput}\nExecution time: ${runResult.executionTimeMs} ms`;
            } else if (runResult.compilationFailed) {
                newConsoleOutput = `Build failed: \n Error List: \n ${runResult.compilationErrors}`
            } else if (runResult.isTimeouted) {
                newConsoleOutput = `Execution timeout, max execution time is: ${runResult.maxExecutionTimeMs} ms`;
            } else if (runResult.isExceptionThrown) {
                newConsoleOutput = `Exception message: \n ${runResult.exceptionMessage}\nStack Trace: \n${runResult.exceptionStackTrace}`;
            }

            self._consoleOutputEditor.setValue(newConsoleOutput);
            self._consoleOutputEditor.setScrollTop(self._consoleOutputEditor.getScrollHeight());
            self._$runButton.button('reset');
            self._$runButton.on('click', function () {
                self._compileAbdRun()
            });
        });
    };

    monaco.languages.registerCompletionItemProvider('csharp', {
        provideCompletionItems: function (model, position) {
            var codeText = model.getValue();
            var absolutePossition = model.getValueLengthInRange(new monaco.Range(0, 0, position.lineNumber, position.column));

            var promise = new Promise(function (resolve) {
                $.ajax({
                    method: "POST",
                    url: "/home/GetRecommendations",
                    data: { code: codeText, index: absolutePossition }
                }).done(function (completions) {
                    resolve(completions);
                });
            });

            return promise;
        }
    });

    $(document).ready(function () {
        var consoleWrapper = document.getElementById('console-wrapper');
        var consoleEmulator = new ConsoleEmulator(consoleWrapper);
    });
});