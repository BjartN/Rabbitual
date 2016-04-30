/// <reference path="typings/handlebars/handlebars.d.ts"/>
/// <reference path="./data.ts"/>
$.urlParam = function (name) {
    var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(window.location.href);
    if (results == null) {
        return null;
    }
    else {
        return results[1] || 0;
    }
};
var Edit = (function () {
    function Edit(e) {
        this.e = e;
        this.data = new Data('http://localhost:9000/config');
        this.updateUrl = 'http://localhost:9000/agent/update';
        this.agentId = $.urlParam('id');
    }
    Edit.prototype.run = function () {
        var that = this;
        var source = document.getElementById('form-template');
        this.tpl = Handlebars.compile(source.innerHTML);
        this.data.get(function (data) {
            var agent = _.find(data, function (x) { return x.id === that.agentId; });
            that.show(agent.options, agent);
        });
    };
    Edit.prototype.show = function (options, agent) {
        var that = this;
        var html = this.tpl(options);
        document.getElementById('app').innerHTML = html;
        $('input').on('blur', function () {
            var newValue = $(this).val();
            var name = $(this).attr('name');
            $.post(that.updateUrl, {
                agentId: that.agentId,
                path: name,
                value: newValue
            });
        });
    };
    return Edit;
}());
var app = new Edit(document.getElementById('app'));
app.run();
