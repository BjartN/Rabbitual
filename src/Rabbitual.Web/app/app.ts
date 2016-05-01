/// <reference path="typings/handlebars/handlebars.d.ts"/>
/// <reference path="./data.ts"/>


class App {
	e: any;
	tableTemplate: any;
	data: DataService;

	constructor(e){
		this.e = e;
		this.data = new DataService()
	}

	run() {
		let that = this;
		let source = document.getElementById('table-template');
		this.tableTemplate = Handlebars.compile(source.innerHTML);

		this.data.get(data=>{
			var html = this.tableTemplate(data);
			document.getElementById('app').innerHTML = html;
		})
	}
}

var app  = new App(document.getElementById('app'));
app.run();
