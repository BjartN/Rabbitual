/// <reference path="typings/handlebars/handlebars.d.ts"/>

class App {
	e: any;
	tableTemplate: any;

	constructor(e){
		this.e = e;
	}

	run() {
		let that = this;
		let source = document.getElementById('table-template');
		this.tableTemplate = Handlebars.compile(source.innerHTML);

		fetch('http://localhost:9000/config').then(r=>{
			r.json().then(that.display.bind(that));
		});
	}

	display(data){
		let props = ['lastCheck', 'lastEventOut', 'lastEventIn', 'lastTaskIn', 'lastTaskOut'];

		_.each(data,x=>{
			_.each(props,p=>{
				if(x[p]){
					x[p] = moment.utc(x[p]).fromNow();
				}
			});
		});

		var html = this.tableTemplate(data);
		document.getElementById('app').innerHTML = html;
	}

}

var app  = new App(document.getElementById('app'));
app.run();
