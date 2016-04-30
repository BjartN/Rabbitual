/// <reference path="typings/handlebars/handlebars.d.ts"/>
/// <reference path="./data.ts"/>


$.urlParam = function(name) {
    var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(window.location.href);
    if (results == null) {
		return null;
    }
    else {
		return results[1] || 0;
    }
}

class Edit {
	e: any;
	tpl: any;
	data: Data;
	updateUrl:string;
	agentId:string;

	constructor(e) {
		this.e = e;
		this.data = new Data('http://localhost:9000/config')
		this.updateUrl = 'http://localhost:9000/agent/update';
		this.agentId =$.urlParam('id');
	}

	run() {

		let that = this;
		let source = document.getElementById('form-template');
		this.tpl = Handlebars.compile(source.innerHTML);

		this.data.get(data => {
			let agent = _.find(data, x => x.id === that.agentId);
			that.show(agent.options,agent);
		})
	}

	show(options,agent) {
		let that = this;
		let html = this.tpl(options);
		document.getElementById('app').innerHTML = html;

		$('input').on('blur',function(){

			let newValue= $(this).val();
			let name = $(this).attr('name');

			$.post(that.updateUrl, {
				agentId : that.agentId,
				path : name,
				value : newValue
			});
		});
	}
}

var app = new Edit(document.getElementById('app'));
app.run();
