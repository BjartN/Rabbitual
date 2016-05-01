/// <reference path="typings/handlebars/handlebars.d.ts"/>
/// <reference path="./data.ts"/>
var App = (function () {
    function App() {
        this.data = new DataService();
    }
    App.prototype.run = function () {
        var that = this;
        var createSource = document.getElementById('create-template');
        this.createTemplate = Handlebars.compile(createSource.innerHTML);
        var source = document.getElementById('table-template');
        this.tableTemplate = Handlebars.compile(source.innerHTML);
        this.data.get(function (data) {
            var html = that.tableTemplate(data);
            document.getElementById('app').innerHTML = html;
        });
        this.data.getAgentTypes(function (data) {
            var html = that.createTemplate(data);
            document.getElementById('create-form').innerHTML = html;
            $('#create-agent').on('click', function () {
                that.data.postAgent($('#agent-name').val(), $('#agent-type').val(), function () {
                    window.location.href = 'index.html';
                });
            });
        });
    };
    return App;
}());
var app = new App();
app.run();
