/// <reference path="typings/handlebars/handlebars.d.ts"/>
var App = (function () {
    function App(e) {
        this.e = e;
    }
    App.prototype.run = function () {
        var that = this;
        var source = document.getElementById('table-template');
        this.tableTemplate = Handlebars.compile(source.innerHTML);
        fetch('http://localhost:9000/config').then(function (r) {
            r.json().then(that.display.bind(that));
        });
    };
    App.prototype.display = function (data) {
        var props = ['lastCheck', 'lastEventOut', 'lastEventIn', 'lastTaskIn', 'lastTaskOut'];
        _.each(data, function (x) {
            _.each(props, function (p) {
                if (x[p]) {
                    x[p] = moment.utc(x[p]).fromNow();
                }
            });
        });
        var html = this.tableTemplate(data);
        document.getElementById('app').innerHTML = html;
    };
    return App;
}());
var app = new App(document.getElementById('app'));
app.run();
