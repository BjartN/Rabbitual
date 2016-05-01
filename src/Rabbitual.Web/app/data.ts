
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
		fetch("http://localhost:9000/agent/update/" +agentId,
			{
				method: "POST",
				body: value
			})
			.then(function(res) { return res.json(); })
			.then(function(data) { callback(data) })
	}
}