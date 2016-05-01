/// <reference path="data.ts"/>

class Editor {
	data: DataService;
	e: any;
	agentId: string;

	constructor(e) {
		this.e = e;
		this.data = new DataService();
		this.agentId = $.urlParam('id');
	}

	createEditor(opt, optSchema) {
		let that = this;
		JSONEditor.defaults.theme = 'foundation5';
		JSONEditor.defaults.iconlib = 'fontawesome4';

		var editor = new JSONEditor(this.e, {
			disable_properties : true,
			disable_edit_json: true,
			disable_collapse: true,
			form_name_root: 'Options',
			startval: opt,
			schema: optSchema
		});

		editor.on('ready', function() {
			editor.validate();
			document.getElementById('submit').addEventListener('click', function() {
				that.data.postOptions(that.agentId, editor.getValue(),function(){})
				console.log(editor.getValue());
			});	
		});

	}

	run(){
		var that = this;
		this.data.getOptions(that.agentId, opt => {
			that.data.getSchema(that.agentId, optSchema => {
				that.createEditor(opt, optSchema);
			})
		});

	}
}

let e = new Editor(document.getElementById('app'));
e.run();