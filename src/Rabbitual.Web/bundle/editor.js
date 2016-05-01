/// <reference path="data.ts"/>
var Editor = (function () {
    function Editor(e) {
        this.e = e;
        this.data = new DataService();
        this.agentId = $.urlParam('id');
    }
    Editor.prototype.createEditor = function (opt, optSchema) {
        var that = this;
        JSONEditor.defaults.theme = 'foundation5';
        JSONEditor.defaults.iconlib = 'fontawesome4';
        var editor = new JSONEditor(this.e, {
            disable_properties: true,
            disable_edit_json: true,
            disable_collapse: true,
            form_name_root: 'Options',
            startval: opt,
            schema: optSchema
        });
        editor.on('ready', function () {
            editor.validate();
            document.getElementById('submit').addEventListener('click', function () {
                that.data.postOptions(that.agentId, editor.getValue(), function () {
                    window.location.href = 'index.html';
                });
            });
        });
    };
    Editor.prototype.run = function () {
        var that = this;
        this.data.getOptions(that.agentId, function (opt) {
            that.data.getSchema(that.agentId, function (optSchema) {
                that.createEditor(opt, optSchema);
            });
        });
    };
    return Editor;
}());
var e = new Editor(document.getElementById('app'));
e.run();
