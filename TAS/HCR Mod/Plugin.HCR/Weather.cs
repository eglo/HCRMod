using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timber_and_Stone;
using Timber_and_Stone.API;
using Timber_and_Stone.API.Event;
using Timber_and_Stone.Event;
using Timber_and_Stone.Blocks;

namespace Plugin.HCR {

	
	public class Weather : MonoBehaviour {
		private static TimeManager tm = AManager<TimeManager>.getInstance();
		private bool isInitialized = false;
		public static int nextRainDay = 0;
		public static int nextRainHour = 0;
		public Vector3i worldSize3i;
		public static bool cheatDone = false;
		
		private static Weather instance = new Weather();			
		public static Weather getInstance() {
			return instance; 
		}
		
		public void Start() {
			if (!Configuration.getInstance().isEnabledWeatherEffects.getBool())
				return;

			gameObject.AddComponent(typeof(Rain));
			gameObject.AddComponent(typeof(RainSound));
			
			Dbg.trc(Dbg.Grp.Startup,3);
			StartCoroutine(doWeather(5.0F));
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
				
		IEnumerator doWeather(float waitTime) {
			int cDay, cHour,secs = 0;

			ChunkManager cm = AManager<ChunkManager>.getInstance();
			while (!isInitialized) {
				worldSize3i = new Vector3i(((cm.worldSize.x) * cm.chunkSize.x),cm.worldSize.y,(cm.worldSize.z) * cm.chunkSize.z);				
				if(worldSize3i.x > 1) {
					Dbg.trc(Dbg.Grp.Weather|Dbg.Grp.Map,1,"worldSize initialized");
					
					gameObject.transform.position = worldSize3i/2;		//TODO: something better
	
					nextRainDay = tm.day+UnityEngine.Random.Range(0,2);
					nextRainHour = tm.hour+UnityEngine.Random.Range(4, 12);
					nextRainHour %= 24;	

					isInitialized = true;
					
				} else {
					Dbg.trc(Dbg.Grp.Weather|Dbg.Grp.Map,1,"worldSize not initialized"+secs.ToString());
					secs++;
				}
				yield return new WaitForSeconds(1.0f);
			}	

			while (true) {
				yield return new WaitForSeconds(waitTime);
				try {		
					cDay = tm.day;
					cHour = tm.hour;

					if ((cDay == 1) && !cheatDone && (UnityEngine.Random.Range(0,200) == 0)) {
						ResourceManager rm = AManager<ResourceManager>.getInstance();
						rm.AddResource(11,5);
						Dbg.printMsg("The woodchopper found some pieces of animal fur. Have any use for these?");
						cheatDone = true;
					}
					if(Rain.getInstance().isRainOnMap && (Time.time >= Rain.getInstance().timeToRemove)) {
						Rain.getInstance().removeRainDrops();
					}

					if((cDay >= nextRainDay) && (cHour >= nextRainHour)) {						
						nextRainDay = cDay;
						nextRainDay += cDay+UnityEngine.Random.Range(0,2);
						nextRainHour = cHour+UnityEngine.Random.Range(4,12);
						nextRainHour %= 24;
						
						int x1 = UnityEngine.Random.Range(1, worldSize3i.x);
						int x2 = UnityEngine.Random.Range(1, worldSize3i.x);
						int z1 = UnityEngine.Random.Range(1, worldSize3i.z);
						int z2 = UnityEngine.Random.Range(1, worldSize3i.z);
						int xpos = Math.Min(x1,x2);
						int xext = Math.Max(x1,x2);
						int zpos = Math.Min(z1,z2);
						int zext = Math.Max(z1,z2);
						switch(UnityEngine.Random.Range(1, 4)) {
							case 1 :
								xpos = 1;
								break;
							case 2 :
								xext = worldSize3i.x;
								break;
							case 3 :
								zpos = 1;
								break;
							case 4 :
								zext = worldSize3i.z;
								break;
						}
						if(Rain.getInstance().isRainOnMap) {
							Rain.getInstance().removeRainDrops();
						}						
						Dbg.msg(Dbg.Grp.Weather,3,"start weather effect over: ", xpos, zpos, xext, zext);
						RainSound rs = (RainSound) gameObject.GetComponent(typeof(RainSound));
						if (rs == null) 
							Dbg.trc(Dbg.Grp.Rain,3,"RainSound :"+"rs == null");
						if (rs.audio == null)
							Dbg.trc(Dbg.Grp.Rain,3,"RainSound :"+"audo == null");
						if (rs.audio.clip == null)
							Dbg.trc(Dbg.Grp.Rain,3,"RainSound :"+"clip == null");
						
						if ((rs == null) || (rs.audio == null) || (rs.audio.clip == null)) {
							Dbg.trc(Dbg.Grp.Rain,3,"RainSound :"+"clip == null");
						} else {
							Dbg.trc(Dbg.Grp.Rain,3,"RainSound :"+audio.clip.ToString());						
							if (!rs.audio.isPlaying && rs.audio.clip.isReadyToPlay) {
								rs.audio.volume = 1.0f;
								rs.audio.loop = true;
								rs.audio.Play();
							}			
						}
						
						rs.isActive = true;
						switch(UnityEngine.Random.Range(1,8)) {
							case 1:	
							case 2:	
								UI.print("It's raining. Good thing all the dirt gets washed away.");
								createRainDrops(xpos, zpos, xext, zext,20);
								lightRain(xpos, zpos, xext, zext);
								break;
								
							case 3:	
							case 4:	
							case 5:	
								UI.print("Lots of rain today. Even those dead trees grow sprouts again");
								createRainDrops(xpos, zpos, xext, zext,30);
								rain(xpos, zpos, xext, zext);
								break;
								
							case 6:	
								UI.print("It's pouring. There's puddles of mud everywhere");
								createRainDrops(xpos, zpos, xext, zext,40);
								rainStorm(xpos, zpos, xext, zext);
								break;
								
								
							default:
								UI.print("Look's like rain is coming ");								
								break;
						}
					}
					Dbg.msg(Dbg.Grp.Time|Dbg.Grp.Weather,2,"Next weather event at " + nextRainDay.ToString() + ":" + nextRainHour.ToString());
				} catch(Exception e) { 
					Dbg.dumpCorExc("doWeather",e);
				}
			}
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		public void rainStorm(int xpos, int zpos, int xext, int zext) {
			ChunkManager cm = AManager<ChunkManager>.getInstance();
			
			Dbg.trc(Dbg.Grp.Map,3);
			rain(xpos, zpos, xext, zext);
			for(int x = xpos; x < xext; x++) {
				for(int z = zpos; z < zext; z++) {
					fillHoles(x, 0, z);
				}
			}
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		public void rain(int xpos, int zpos, int xext, int zext) {
			
			Dbg.trc(Dbg.Grp.Map,3);
			lightRain(xpos, zpos, xext, zext);
			for(int x = xpos; x < xext; x++) {
				for(int z = zpos; z < zext; z++) {
					regrowTrees(x, 0, z);
				}
			}
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		public void lightRain(int xpos, int zpos, int xext, int zext) {
			
			Dbg.trc(Dbg.Grp.Map,3);
			for(int x = xpos; x < xext; x++) {
				for(int z = zpos; z < zext; z++) {
					replaceBlock(x, 0, z, BlockProperties.BlockBurnt, BlockProperties.BlockDirt);
				}
			}
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		private void fillHoles(int x, int y, int z) {
			IBlock topBlk;
			IBlock checkBlk;
			IBlock newBlk;
			int topBlkID;
			int surround = 0;
			
			Dbg.trc(Dbg.Grp.Map,1,2);
			
			ChunkManager cm = AManager<ChunkManager>.getInstance();			
			topBlk = cm.GetBlockOnTop(Coordinate.FromBlock(x, 0, z));
			topBlkID = topBlk.properties.getID();
			for(int cx = -1; cx <=1; cx++) {
				for(int cz = -1; cz <=1; cz++) {
					checkBlk = topBlk.relative(cx, +1, cz);
					if(
						(checkBlk.properties.getID() != BlockProperties.BlockAir.getID()) &&
						(checkBlk.properties.getID() != BlockProperties.SlopeStone.getID()) &&
						(checkBlk.properties.getID() != BlockProperties.SlopeGrass.getID()) &&
						(checkBlk.properties.getID() != BlockProperties.SlopeDirt.getID()) &&
						(checkBlk.properties.getID() != BlockProperties.SlopeSand.getID())
						) {
						surround++;
					}
				}
			}
			if(surround >= 6) {
				Dbg.trc(Dbg.Grp.Map,3,2);
				newBlk = topBlk.relative(0, +1, 0);
				cm.SetBlock(newBlk.coordinate, BlockProperties.BlockDirt);
				Dbg.msg(Dbg.Grp.Map,1,"Filled a hole on top of a " + topBlk.properties.ToString() + " at " + x.ToString() + "," + z.ToString());
			}
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		private void regrowTrees(int x, int y, int z) {

//			TerrainObjectManager tm = AManager<TerrainObjectManager>.getInstance();			
//			foreach (TreeFlora treeObj in tm.treeObjects) {
//				if(
//					(treeObj.transform.position.x == x) 
//					(treeObj.transform.position.y == y) 
//				)
//					treeObj.onFire = false;
//						
//			}
			
			
//		ChunkManager cm = AManager<ChunkManager>.getInstance();			
//			BlockProperties checkProps = BlockProperties.BlockTreeBase;
//			BlockProperties replaceProps = BlockProperties.BlockTreeBaseBurnt;
//			IBlock topBlk;
//						
//			Dbg.trc(Dbg.Grp.Map,1);
//			topBlk = cm.GetBlockOnTop(Coordinate.FromBlock(x, 0, z));
//			if(topBlk.properties == checkProps) {
//				cm.SetBlock(topBlk.coordinate, BlockProperties.BlockTreeBase);
//				Dbg.msg(Dbg.Grp.Map,2,"Replaced type " + topBlk.properties.ToString() + " with " + replaceProps.ToString() + " at " + x.ToString() + "," + z.ToString());
//
//				Transform transform = UnityEngine.Object.Instantiate(AManager<AssetManager>.getInstance().tree, base.transform.position, Quaternion.identity) as Transform;
//				transform.transform.parent = AManager<ChunkManager>.getInstance().chunkArray[topBlk.coordinate.chunk.x, topBlk.coordinate.chunk.y, topBlk.coordinate.chunk.z].chunkObj.transform;
//				transform.GetComponent<TreeFlora>().blockPos = topBlk.coordinate.block;
//				transform.GetComponent<TreeFlora>().chunkPos = topBlk.coordinate.chunk;
//				AManager<TerrainObjectManager>.getInstance().AddTree(transform.GetComponent<TreeFlora>());
//				transform.GetComponent<TreeFlora>().health = 61f;
//				transform.GetComponent<TreeFlora>().Init();
//			}
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////

		private void createRainDrops(int xpos, int zpos, int xext, int zext, int dropRate) {
			
			if (Configuration.getInstance().isEnabledShowRainBlocks.get() == 0)
				return;
			
			ChunkManager cm = AManager<ChunkManager>.getInstance();
			IBlock topBlk;
			IBlock newBlk;
			
			Dbg.trc(Dbg.Grp.Map,2,1);			
			for(int x = xpos; x < xext; x++) {
				for(int z = zpos; z < zext; z++) {
					if(UnityEngine.Random.Range(0, 200-dropRate) != 1)
						continue;

					Dbg.trc(Dbg.Grp.Map,2,2);
					topBlk = getBlockOnTop(Coordinate.FromBlock(x, worldSize3i.y - 1, z));
					int height = UnityEngine.Random.Range(10,20);
					if((topBlk.coordinate.absolute.y+height) >= (worldSize3i.y-1))
						continue;
					newBlk = topBlk.relative(0, height, 0);
					Dbg.msg(Dbg.Grp.Map,1,"New raindrop over" + topBlk.coordinate.ToString());
					Rain.getInstance().addRainDrop(newBlk.coordinate.world,topBlk.coordinate.world);
					//rainDrops.Add(newBlk.coordinate);
				}
			}
			//isRainOnMap = true;
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
//kill this?
//		private void removeRainDrops() {
//			ChunkManager cm = AManager<ChunkManager>.getInstance();
//			
//			Dbg.trc(Dbg.Grp.Map,2);
//			
//			foreach(Coordinate rainDrop in rainDrops) {
//				cm.SetBlock(rainDrop, BlockProperties.BlockAir);
//			}
//			rainDrops.Clear();
//			isRainOnMap = false;
//		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		/// set a block regardless of surrounding or what was there before
//kill this?
//		private void xxxputBlock(int x, int y, int z, BlockProperties blockPropsNew) { try {
//				IBlock oldBlk;
//				IBlock newBlk;
//				
//				GUIManager gm = AManager<GUIManager>.getInstance();
//				ChunkManager cm = AManager<ChunkManager>.getInstance();
//				oldBlk = cm.GetBlock(Coordinate.FromBlock(x, y, z));
//				newBlk = oldBlk.relative(0, 0, 0);
//				cm.SetBlock(newBlk.coordinate, blockPropsNew);
//				Dbg.msg(Dbg.Grp.Map,1,"Set type " + blockPropsNew.ToString() + " in place of a " + oldBlk.properties.ToString() + " at " + x.ToString() + "," + z.ToString());
//				
//			} catch(Exception e) {Dbg.dumpExc(e);}}
		
		
		///////////////////////////////////////////////////////////////////////////////////////////
		/// put a block on top of map at coordinate, ignores height y, replaces air
		private void putBlockOnTop(int x, int y, int z, BlockProperties blockPropsNew) { //try {
			ChunkManager cm = AManager<ChunkManager>.getInstance();
			IBlock topBlk;
			IBlock newBlk;
			
			Dbg.trc(Dbg.Grp.Map,1);
			topBlk = cm.GetBlockOnTop(Coordinate.FromBlock(x, 0, z));
			newBlk = topBlk.relative(0, +1, 0);
			if(cm.isCoordInMap(newBlk.coordinate)) {
				cm.SetBlock(newBlk.coordinate, blockPropsNew);
			} else {
				Dbg.msg(Dbg.Grp.Map,10,"Could not put Block on top at " + x.ToString() + "," + y.ToString() + "," + z.ToString());
			}
			Dbg.msg(Dbg.Grp.Map,1,"Set type " + blockPropsNew.ToString() + " on top of a " + topBlk.properties.ToString() + " at " + x.ToString() + "," + z.ToString());
			
		} //catch (Exception e) {exceptionPrint(e);}}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		/// replace blocktype old with new, dont do anything when no blocktype old at coordinate
		private void replaceBlock(int x, int y, int z, BlockProperties blockPropsOld, BlockProperties blockPropsNew) { // try {
			ChunkManager cm = AManager<ChunkManager>.getInstance();
			IBlock topBlk;
			
			Dbg.trc(Dbg.Grp.Map,1);
			topBlk = cm.GetBlockOnTop(Coordinate.FromBlock(x, 0, z));
			if(topBlk.properties == blockPropsOld) {
				cm.SetBlock(topBlk.coordinate, blockPropsNew);
				Dbg.msg(Dbg.Grp.Map,1,"Replaced type " + topBlk.properties.ToString() + " with " + blockPropsNew.ToString() + " at " + x.ToString() + "," + z.ToString());
			}
		}  // catch (Exception e) {exceptionPrint(e);}}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		// get highest non-air block
		// ChunkManager.getBlockOnTop doesn't seem to work when searching from air/top down for me, 
		// not sure if borken or I am using it wrong? 
		private IBlock getBlockOnTop(Coordinate coord) { // try {
			ChunkManager cm = AManager<ChunkManager>.getInstance();
			IBlock topBlk;
			
			Dbg.trc(Dbg.Grp.Map,1);
			topBlk = cm.GetBlock(coord);
			while(topBlk.properties.getID() == BlockProperties.BlockAir.getID()) {
				topBlk = topBlk.relative(0, -1, 0);
			}
			;
			
			return topBlk;
		}  // catch (Exception e) {exceptionPrint(e);}}
		
		
		///////////////////////////////////////////////////////////////////////////////////////////
		///////////////////////////////////////////////////////////////////////////////////////////
				
		
		//place markers on map as a debugging aid, not used atm
		private void placeMapMarkers() {
			for(int i = 0; i < 3; i++)
				putBlockOnTop(0, 0, 0, BlockProperties.BlockIronOre);
			for(int i = 0; i < 3; i++)
				putBlockOnTop(worldSize3i.x, 0, 0, BlockProperties.BlockSilverOre);
			for(int i = 0; i < 3; i++)
				putBlockOnTop(0, 0, worldSize3i.z, BlockProperties.BlockGoldOre);
			for(int i = 0; i < 3; i++)
				putBlockOnTop(worldSize3i.x, 0, worldSize3i.z, BlockProperties.BlockMithrilOre);
			//for (int i = 1; i < worldSize3i.x; i++)
			//	putBlockOnTop (i, 0, i, BlockProperties.BlockCopperOre.getID());
		}
		
	}
}

