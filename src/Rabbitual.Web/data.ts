
 class Data {
	url: String;

	constructor(url) {
		this.url = url;
	}

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
}