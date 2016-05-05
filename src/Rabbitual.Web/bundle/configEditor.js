/// <reference path="../typings/tsd.d.ts"/>
var ConfigEditor = (function () {
    function ConfigEditor() {
        this.data = new DataService();
        this.agentId = $.urlParam('id');
    }
    ConfigEditor.prototype.run = function () {
        var _this = this;
        var createSource = document.getElementById('form-template');
        this.tpl = Handlebars.compile(createSource.innerHTML);
        this.data.getConfig(function (data) {
            var agent = _.find(data, function (x) { return x.id === _this.agentId; });
            var html = _this.tpl(agent);
            document.getElementById('app').innerHTML = html;
            $('#submit').on('click', function () {
                _this.data.postConfig(_this.agentId, { name: $('#name').val(), sourceIds: $('#sources').val().split(',') }, function () {
                    window.location.href = 'index.html';
                });
            });
        });
    };
    return ConfigEditor;
}());
new ConfigEditor().run();
