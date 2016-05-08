/// <reference path="../typings/tsd.d.ts"/>
var ConfigEditor = (function () {
    function ConfigEditor() {
        this.data = new DataService();
        this.agentId = parseInt($.urlParam('id'));
    }
    ConfigEditor.prototype.run = function () {
        var _this = this;
        var createSource = document.getElementById('form-template');
        this.tpl = Handlebars.compile(createSource.innerHTML);
        this.data.getConfig(function (data) {
            var all = data;
            var cfg = _.find(data, function (x) { return x.id === _this.agentId; });
            var html = _this.tpl({ cfg: cfg, all: all });
            $('#app').html(html);
            var $d = $('.msources').dropdown();
            _.each(cfg.sources, function (x) { return $d.dropdown('set selected', x); });
            $('#submit').on('click', function () {
                _this.data.postConfig(_this.agentId, {
                    name: $('#name').val(),
                    sourceIds: $d.dropdown('get value')
                }, function () {
                    window.location.href = 'index.html';
                });
            });
        });
    };
    return ConfigEditor;
}());
new ConfigEditor().run();
