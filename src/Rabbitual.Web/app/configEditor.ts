/// <reference path="../typings/tsd.d.ts"/>

class ConfigEditor {
	private data: DataService;
	private agentId: number;
	private tpl: any;

	constructor(){
		this.data = new DataService();
		this.agentId = parseInt($.urlParam('id'));
	}

	run(){

		let createSource = document.getElementById('form-template');
		this.tpl = Handlebars.compile(createSource.innerHTML);

		this.data.getConfig(data => {
			var all = data;
			var cfg = _.find(data, x => x.id === this.agentId);

			var html = this.tpl({cfg:cfg,all:all});
			$('#app').html(html);
			var $d = $('.msources').dropdown();


			_.each(cfg.sources, x => $d.dropdown('set selected', x));
			$('#submit').on('click', () => {
				this.data.postConfig(this.agentId, 
					{ 
						name: $('#name').val(), 
						sourceIds: $d.dropdown('get value')
					}, 
					() => {window.location.href = 'index.html'
				})
			});	

		});

	}
}

new ConfigEditor().run();