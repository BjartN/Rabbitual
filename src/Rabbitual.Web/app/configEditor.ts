/// <reference path="../typings/tsd.d.ts"/>

class ConfigEditor {
	private data: DataService;
	private agentId: string;
	private tpl: any;

	constructor(){
		this.data = new DataService();
		this.agentId = $.urlParam('id');
	}

	run(){

		let createSource = document.getElementById('form-template');
		this.tpl = Handlebars.compile(createSource.innerHTML);

		this.data.getConfig(data => {
			var agent = _.find(data, x => x.id === this.agentId);
			var html = this.tpl(agent);
			document.getElementById('app').innerHTML = html;


			$('#submit').on('click', () => {
				this.data.postConfig(this.agentId, {  name: $('#name').val(), sourceIds: $('#sources').val().split(',') }, () => {
					window.location.href = 'index.html'
				})
			});	

		});

	}
}

new ConfigEditor().run();