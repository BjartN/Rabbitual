var DataService = (function () {
    function DataService() {
    }
    DataService.prototype.get = function (callback) {
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
    DataService.prototype.getSchema = function (agentId, callback) {
        fetch('http://localhost:9000/agent/options/schema/' + agentId).then(function (r) {
            r.json().then(function (data) {
                callback(data);
            });
        });
    };
    DataService.prototype.getOptions = function (agentId, callback) {
        fetch('http://localhost:9000/agent/options/' + agentId).then(function (r) {
            r.json().then(function (data) {
                callback(data);
            });
        });
    };
    DataService.prototype.postOptions = function (agentId, value, callback) {
        var bodz = JSON.stringify(value);
        $.ajax({
            type: 'POST',
            url: 'http://localhost:9000/agent/options/update/' + agentId,
            data: bodz,
            success: callback
        });
    };
    return DataService;
}());
