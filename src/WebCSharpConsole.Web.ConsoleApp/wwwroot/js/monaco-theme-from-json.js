var rules = (function () {
    var themeSettings = [
        {
            "name": "Function declarations",
            "scope": [
                "entity.name.function",
                "support.function",
                "support.constant.handlebars"
            ],
            "settings": {
                "foreground": "#DCDCAA"
            }
        },
        {
            "name": "Types declaration and references",
            "scope": [
                "meta.return-type",
                "support.class",
                "support.type",
                "entity.name.type",
                "entity.name.class",
                "storage.type.cs",
                "storage.type.generic.cs",
                "storage.type.modifier.cs",
                "storage.type.variable.cs",
                "storage.type.annotation.java",
                "storage.type.generic.java",
                "storage.type.java",
                "storage.type.object.array.java",
                "storage.type.primitive.array.java",
                "storage.type.primitive.java",
                "storage.type.token.java",
                "storage.type.groovy",
                "storage.type.annotation.groovy",
                "storage.type.parameters.groovy",
                "storage.type.generic.groovy",
                "storage.type.object.array.groovy",
                "storage.type.primitive.array.groovy",
                "storage.type.primitive.groovy"
            ],
            "settings": {
                "foreground": "#4EC9B0"
            }
        },
        {
            "name": "Types declaration and references, TS grammar specific",
            "scope": [
                "meta.type.cast.expr",
                "meta.type.new.expr",
                "support.constant.math",
                "support.constant.dom",
                "support.constant.json",
                "entity.other.inherited-class"
            ],
            "settings": {
                "foreground": "#4EC9B0"
            }
        },
        {
            "name": "Control flow keywords",
            "scope": "keyword.control",
            "settings": {
                "foreground": "#C586C0"
            }
        },
        {
            "name": "Variable and parameter name",
            "scope": [
                "variable",
                "meta.definition.variable.name",
                "support.variable"
            ],
            "settings": {
                "foreground": "#9CDCFE"
            }
        },
        {
            "name": "Object keys, TS grammar specific",
            "scope": [
                "meta.object-literal.key",
                "meta.object-literal.key entity.name.function"
            ],
            "settings": {
                "foreground": "#9CDCFE"
            }
        },
        {
            "name": "CSS property value",
            "scope": [
                "support.constant.property-value",
                "support.constant.font-name",
                "support.constant.media-type",
                "support.constant.media",
                "constant.other.color.rgb-value",
                "constant.other.rgb-value",
                "support.constant.color"
            ],
            "settings": {
                "foreground": "#CE9178"
            }
        }
    ];

    var themeRules = [];

    themeSettings.forEach(function (item) {
        var tokens;

        if (item.scpoe instanceof Array) {
            tokens = item.scope;
        } else {
            tokens = [item.scope];
        }

        tokens.forEach(function (token) {
            var rule = { token: token };

            if (item.settings.foreground) {
                var foreground = item.settings.foreground.replace('#', '');
                rule.foreground = foreground;
            }

            if (item.settings.background) {
                var background = item.settings.background.replace('#', '');
                rule.background = background;
            }

            if (item.settings.fontStyle) {
                rule.fontStyle = item.settings.fontStyle;
            }

            themeRules.push(rule);
        });
    });

    return themeRules;
})();

monaco.editor.defineTheme('myCustomTheme', {
    base: 'vs-dark', // can also be vs-dark or hc-black
    inherit: true, // can also be false to completely replace the builtin rules
    rules: rules
});