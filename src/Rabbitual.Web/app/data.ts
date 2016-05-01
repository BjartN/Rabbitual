
 class DataService {
	
	get(callback) {
		let that = this;
		fetch('http://localhost:9000/config').then(r => {
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
		fetch('http://localhost:9000/agent/options/schema/' + agentId).then(r => {
			r.json().then(data => {
				callback(data);
			});
		});
	}

	getOptions(agentId, callback) {
		fetch('http://localhost:9000/agent/options/' + agentId).then(r => {
			r.json().then(data => {
				callback(data);
			});
		});
	}

	postOptions(agentId,value,callback){

		var bodz = JSON.stringify(value);

		$.ajax({
			type: 'POST',
			url: 'http://localhost:9000/agent/options/update/' + agentId,
			data: bodz,
			success: callback
		});

		
	}
}