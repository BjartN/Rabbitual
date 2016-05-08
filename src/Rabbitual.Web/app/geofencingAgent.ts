/// <reference path="../typings/tsd.d.ts"/>
/// <reference path="data.ts"/>

class GeofencingAgent {

	private data: DataService;
	private agentId: number;

	constructor(){
		this.data = new DataService();
		this.agentId = $.urlParam('id');
	}

	run (){

		var map = L.map($('.map')[0]).setView([60.397076, 5.324383], 12);
		L.tileLayer('http://a.tiles.mapbox.com/v3/stormgeo.gik0ohaj/{z}/{x}/{y}.png', {
			tms: false
		}).addTo(map);

		this.data.getConfig(data =>{

			var fences = _.chain(data)
			.filter(x => x.agentType === 'GeofencingAgent')
			.map(x => { 
				return {
					lat:x.options.circleFence.lat,
					lon: x.options.circleFence.lon,
					radiusMeters: x.options.circleFence.radiusMeters,
					id: x.id
				}
			})
			.value();

			_.forEach(fences, x=>{
				L.circle([x.lat,x.lon], x.radiusMeters).addTo(map);
			});
		})
	}
}


let a = new GeofencingAgent();
a.run();