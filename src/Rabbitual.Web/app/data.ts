/// <reference path="../typings/tsd.d.ts"/>

 class DataService {
	 root: string

 	constructor(){
		  this.root = 'http://localhost:9000'
 	}

	get(callback) {
		let that = this;
		fetch(`${this.root}/config`).then(r => {
			r.json().then(data=>{
				let props = ['lastCheck', 'lastEventOut', 'lastEventIn', 'lastTaskIn', 'lastTaskOut'];
				_.each(data, x => {
					_.each(props, p => {
						if (x[p]) {
							x[p] = moment.utc(x[p]).fromNow();
						}
					});
				});
				
				callback(data);				
			});
		});
	}

	getSchema(agentId,callback) {
		fetch(`${this.root}/agent/options/schema/` + agentId).then(r => {
			r.json().then(data => {
				callback(data);
			});
		});
	}

	getOptions(agentId, callback) {
		fetch(`${this.root}/agent/options/` + agentId).then(r => {
			r.json().then(data => {
				callback(data);
			});
		});
	}

	postAgent(name, type, callback) {

		$.ajax({
			type: 'POST',
			data: {name:name,type:type},
			url: `${this.root}/agent-create`,
			success: callback
		});
	}

	getAgentTypes(callback) {

		$.ajax({
			type: 'GET',
			url: `${this.root}/agent/types`,
			success: callback
		});

	}

	postOptions(agentId,value,callback){

		var bodz = JSON.stringify(value);

		$.ajax({
			type: 'POST',
			url: `${this.root}/agent/options/update/` + agentId,
			data: bodz,
			success: callback
		});
	}
}