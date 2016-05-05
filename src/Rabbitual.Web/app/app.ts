/// <reference path="../typings/tsd.d.ts"/>
/// <reference path="data.ts"/>

class App {
	tableTemplate: any;
	createTemplate: any;
	data: DataService;

	constructor(){
		this.data = new DataService()
	}

	run() {
		let that = this;

		let createSource = document.getElementById('create-template');
		this.createTemplate = Handlebars.compile(createSource.innerHTML);

		let source = document.getElementById('table-template');
		this.tableTemplate = Handlebars.compile(source.innerHTML);

		this.data.getConfig(data=>{
			var html = that.tableTemplate(data);
			document.getElementById('app').innerHTML = html;
		})

		this.data.getAgentTypes(data=>{
			var html = that.createTemplate(data);
			document.getElementById('create-form').innerHTML = html;

			$('#create-agent').on('click', () => {
				that.data.postAgent($('#agent-name').val(), $('#agent-type').val(), () => {
					window.location.href = 'index.html'
				});
			});
		});
	}
}

var app  = new App();
app.run();
