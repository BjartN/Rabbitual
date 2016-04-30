/// <reference path="typings/handlebars/handlebars.d.ts"/>
/// <reference path="./data.ts"/>
var App = (function () {
    function App(e) {
        this.e = e;
        this.data = new Data('http://localhost:9000/config');
    }
    App.prototype.run = function () {
        var _this = this;
        var that = this;
        var source = document.getElementById('table-template');
        this.tableTemplate = Handlebars.compile(source.innerHTML);
        this.data.get(function (data) {
            var html = _this.tableTemplate(data);
            document.getElementById('app').innerHTML = html;
        });
    };
    return App;
}());
var app = new App(document.getElementById('app'));
app.run();
