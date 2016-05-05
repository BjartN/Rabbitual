/// <reference path="../typings/tsd.d.ts"/>

 class DataService {
	 root: string

 	constructor(){
		  this.root = 'http://localhost:9000'
 	}

 	getAgentOptions(agentId,callback){
		  this.get(`${this.root}/agent/options/${agentId}`, callback);
 	}

	getConfig(callback) {
		let that = this;
		this.get(`${this.root}/config`,data => {
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
	}

	getSchema(agentId,callback) {
		this.get(`${this.root}/agent/options/schema/` + agentId,callback);
	}

	getFatOptions(agentId, callback) {
		this.get(`${this.root}/agent/fat-options/` + agentId,callback);
	}

	getOptions(agentId, callback) {
		this.get(`${this.root}/agent/options/` + agentId, callback);
	}

	getAgentTypes(callback) {
		$.ajax({
			type: 'GET',
			url: `${this.root}/agent/types`,
			success: callback
		});
	}

	postOptions(agentId,value,callback){
		this.post(`${this.root}/agent/options/update/` + agentId, JSON.stringify(value), callback);
	}

	postConfig(agentId, value, callback) {
		this.post(`${this.root}/agent/config/update/` + agentId, JSON.stringify(value), callback);
	}

	postAgent(name, type, callback) {
		this.post(`${this.root}/agent-create`, { name: name, type: type }, callback);
	}

	post(url, data, callback) {
		$.ajax({
			type: 'POST',
			data: data,
			url: url,
			success: callback
		});
	}

	get(url, callback) {
		$.ajax({
			type: 'GET',
			url: url,
			success: callback
		});
	}

}