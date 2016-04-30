var Data = (function () {
    function Data(url) {
        this.url = url;
    }
    Data.prototype.get = function (callback) {
        var that = this;
        fetch('http://localhost:9000/config').then(function (r) {
            r.json().then(function (data) {
                var props = ['lastCheck', 'lastEventOut', 'lastEventIn', 'lastTaskIn', 'lastTaskOut'];
                _.each(data, function (x) {
                    _.each(props, function (p) {
                        if (x[p]) {
                            x[p] = moment.utc(x[p]).fromNow();
                        }
                    });
                });
                callback(data);
            });
        });
    };
    return Data;
}());
