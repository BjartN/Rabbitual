/// <reference path="../typings/tsd.d.ts"/>
/// <reference path="data.ts"/>
var GeofencingAgent = (function () {
    function GeofencingAgent() {
        this.data = new DataService();
        this.agentId = $.urlParam('id');
    }
    GeofencingAgent.prototype.run = function () {
        var map = L.map($('.map')[0]).setView([60.397076, 5.324383], 12);
        L.tileLayer('http://a.tiles.mapbox.com/v3/stormgeo.gik0ohaj/{z}/{x}/{y}.png', {
            tms: false
        }).addTo(map);
        this.data.getConfig(function (data) {
            var fences = _.chain(data)
                .filter(function (x) { return x.agentType === 'GeofencingAgent'; })
                .map(function (x) {
                return {
                    lat: x.options.circleFence.lat,
                    lon: x.options.circleFence.lon,
                    radiusMeters: x.options.circleFence.radiusMeters,
                    id: x.id
                };
            })
                .value();
            _.forEach(fences, function (x) {
                L.circle([x.lat, x.lon], x.radiusMeters).addTo(map);
            });
        });
    };
    return GeofencingAgent;
}());
var a = new GeofencingAgent();
a.run();
