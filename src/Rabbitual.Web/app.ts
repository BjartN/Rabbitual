/// <reference path="typings/handlebars/handlebars.d.ts"/>
/// <reference path="./data.ts"/>


class App {
	e: any;
	tableTemplate: any;
	data: Data;

	constructor(e){
		this.e = e;
		this.data = new Data('http://localhost:9000/config')
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
