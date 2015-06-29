using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Timber_and_Stone;
using Timber_and_Stone.API;
using Timber_and_Stone.API.Event;
using Timber_and_Stone.Event;
using Timber_and_Stone.Blocks;

namespace Plugin.HCR {
	public class PluginMain : CSharpPlugin, IEventListener {


		private static PluginMain instance;
		public static PluginMain getInstance() {
			return instance;
		}

		public override void OnLoad() {
			instance = this;
			EventManager.getInstance().Register(this);			
		}

		public override void OnEnable() {
			AManager<WorldManager>.getInstance().gameObject.AddComponent(typeof(HCRMod));
		}

		public override void OnDisable() {
		}

		[Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
		public void onMigrant(EventMigrant evt) {
			Configuration conf = Configuration.getInstance();
			if(conf.isEnabledMoreImmigrants.getBool()) {
				MoreImmigrants.processEvent(ref evt);
			}			
		}

		[Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
		public void onMigrantAccept(EventMigrantAccept evt) {
		}

		[Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
		public void onEventCraft(EventCraft evt) {
		}

		[Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
		public void onMine(EventMine evt) {
		}

		[Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
		public void onEventGrow(EventBlockGrow evt) {
		}

		[Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
		public void onEventBlockSet(EventBlockSet evt) {
		}

		[Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
		public void onEventBuildStructure(EventBuildStructure evt) {
		}

		[Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
		public void onInvasionNormal(EventInvasion evt) {	
			Configuration conf = Configuration.getInstance();
			if(!conf.isEnabledInvasionConfig.getBool()) {
				evt.result = Result.Allow;
				return;
			} else {
				InvasionHandler.processEvent(ref evt);
			}
		}

		[Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
		public void onInvasionMonitor(EventInvasion evt) {
			Result arg_0F_0 = evt.result;
		}
	}
}






