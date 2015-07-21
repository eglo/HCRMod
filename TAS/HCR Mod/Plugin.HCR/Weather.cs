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

	
	public class Weather : SingletonEntity<Weather> {
		private bool isInitialized = false;
		public int nextRainDay = 0;
		public int nextRainHour = 0;
		public Vector3i worldSize3i;


		public override void Awake() {
			Dbg.trc(Dbg.Grp.Init, 5);
		}
		
		public void Start() {
			Dbg.trc(Dbg.Grp.Startup, 5);

			if(Configuration.getInstance().isEnabledShowRainBlocks.getBool()) {
				AddEntity<Rain>(this.transform);
			}
			
			StartCoroutine(doWeather(5.0F));
		}
		
		//*****************************************************************************************
				
		IEnumerator doWeather(float waitTime) {

			ChunkManager cm = AManager<ChunkManager>.getInstance();
			TimeManager tm = AManager<TimeManager>.getInstance();
			int cDay, cHour, secs = 0;

			while(!isInitialized) {
				worldSize3i = new Vector3i(((cm.worldSize.x) * cm.chunkSize.x),cm.worldSize.y,(cm.worldSize.z) * cm.chunkSize.z);				
				if(worldSize3i.x > 1) {
					Dbg.trc(Dbg.Grp.Weather | Dbg.Grp.Terrain, 3, "worldSize initialized");					
					gameObject.transform.position = worldSize3i / 2;		//TODO: something better	
					nextRainDay = tm.day + UnityEngine.Random.Range(0, 2);
					nextRainHour = tm.hour + UnityEngine.Random.Range(4, 12);
					nextRainHour %= 24;	

					isInitialized = true;					
				} else {
					Dbg.trc(Dbg.Grp.Weather | Dbg.Grp.Terrain, 3, "worldSize not initialized" + secs.ToString());
					secs++;
				}
				yield return new WaitForSeconds(waitTime);
			}	

			while(true) {
				yield return new WaitForSeconds(waitTime);
				
				try {		
					cDay = tm.day;
					cHour = tm.hour;

					Rain rs = GetEntity<Rain>();
					if(rs.isRainOnMap && (Time.time >= rs.timeToRemove)) {
						rs.removeRain();
					}

					if((cDay >= nextRainDay) && (cHour >= nextRainHour) && !rs.isRainOnMap) {						
						nextRainDay = cDay + UnityEngine.Random.Range(0, 3);
						nextRainHour = cHour + UnityEngine.Random.Range(8, 16);
						nextRainHour %= 24;
						
						int xpos = 1, zpos = 1, xext = worldSize3i.x, zext = worldSize3i.z;
						Dbg.msg(Dbg.Grp.Weather, 5, "start weather effect over: ", xpos, zpos, xext, zext);
						switch(UnityEngine.Random.Range(1, 8)) {
							case 1:	
							case 2:	
								UI.print("It's raining. Good thing all the dirt gets washed away.");
								StartCoroutine(createRainDrops(xpos, zpos, xext, zext, 1));
								StartCoroutine(doRemoveBurntDirt(xpos, zpos, xext, zext));
								break;
								
							case 3:	
							case 4:	
							case 5:	
								UI.print("Lots of rain today. Even those dead trees are growing sprouts again");
								StartCoroutine(createRainDrops(xpos, zpos, xext, zext, 2));
								StartCoroutine(doRemoveBurntDirt(xpos, zpos, xext, zext));
								StartCoroutine(doRegrowTrees(xpos, zpos, xext, zext));
								break;
								
							case 6:	
							case 7:	
								UI.print("It's pouring. The muddy water fills up puddles everywhere");
								StartCoroutine(createRainDrops(xpos, zpos, xext, zext, 3));
								StartCoroutine(doRemoveBurntDirt(xpos, zpos, xext, zext));
								StartCoroutine(doRegrowTrees(xpos, zpos, xext, zext));
								StartCoroutine(doFillHoles(xpos, zpos, xext, zext));
								break;
								
								
							default:
								UI.print("Look's like rain is coming ");								
								break;
						}
					}
					Dbg.msg(Dbg.Grp.Time | Dbg.Grp.Weather, 4, "Next weather event at " + nextRainDay.ToString() + ":" + nextRainHour.ToString());
				} catch(Exception e) { 
					Dbg.dumpCorExc("doWeather", e);
				}
			}
		}
		
		//*****************************************************************************************
		public IEnumerator doFillHoles(int xpos, int zpos, int xext, int zext) {
			ChunkManager cm = AManager<ChunkManager>.getInstance();

			Dbg.trc(Dbg.Grp.Terrain, 5);
			for(int x = xpos; x < xext; x++) {
				for(int z = zpos; z < zext; z++) {
					fillHoles(x, 0, z);
				}
				yield return null;
			}
		}
		
		//*****************************************************************************************
		public IEnumerator doRegrowTrees(int xpos, int zpos, int xext, int zext) {
			
			Dbg.trc(Dbg.Grp.Terrain, 5);

			regrowTrees(0, 0, 0);

			yield return null;
		}
		
		//*****************************************************************************************
		private void regrowTrees(int x, int y, int z) {
			
			Dbg.trc(Dbg.Grp.Terrain, 4);
			
			TerrainObjectManager tm = AManager<TerrainObjectManager>.getInstance();			
			foreach (TreeFlora treeObj in tm.treeObjects) {
				if(
					(treeObj.onFire == true)
					//&& (treeObj.transform.position.x == x)
					//&& (treeObj.transform.position.y == y) 
					) {
					treeObj.onFire = false;
					treeObj.stage = "seed";
					treeObj.health = 30;
				}
			}	
		}
		//*****************************************************************************************
		public IEnumerator doRemoveBurntDirt(int xpos, int zpos, int xext, int zext) {
			
			Dbg.trc(Dbg.Grp.Terrain, 5);
			for(int x = xpos; x < xext; x++) {
				for(int z = zpos; z < zext; z++) {
					replaceBlock(x, 0, z, BlockProperties.BlockBurnt, BlockProperties.BlockDirt);
					if (UnityEngine.Random.Range(0,11) == 0) {
						replaceBlock(x, 0, z, BlockProperties.BlockDirt, BlockProperties.BlockGrass);
					}
				}
				yield return null;
			}
		}
		
		//*****************************************************************************************

		private void fillHoles(int x, int y, int z) {
			IBlock topBlk;
			IBlock checkBlk;
			IBlock newBlk;
			int topBlkID;
			float surround = 0.0f;
			
			Dbg.trc(Dbg.Grp.Terrain, 4, "start");
			
			ChunkManager cm = AManager<ChunkManager>.getInstance();			
			topBlk = getBlockOnTop(Coordinate.FromBlock(x, worldSize3i.y - 1, z));
			topBlkID = topBlk.properties.getID();
			for(int cx = -1; cx <=1; cx++) {
				for(int cz = -1; cz <=1; cz++) {
					checkBlk = topBlk.relative(cx, +1, cz);
					if(
						(checkBlk.properties.getID() == BlockProperties.BlockStone.getID()) ||
						(checkBlk.properties.getID() == BlockProperties.BlockGrass.getID()) ||
						(checkBlk.properties.getID() == BlockProperties.BlockDirt.getID()) ||
						(checkBlk.properties.getID() == BlockProperties.BlockSand.getID())
						) {
						surround += 1.0f;
					}
					if(
						(checkBlk.properties.getID() == BlockProperties.SlopeStone.getID()) ||
						(checkBlk.properties.getID() == BlockProperties.SlopeGrass.getID()) ||
						(checkBlk.properties.getID() == BlockProperties.SlopeDirt.getID()) ||
						(checkBlk.properties.getID() == BlockProperties.SlopeSand.getID())
						) {
						surround += 0.5f;
					}
				}
			}
			if(surround >= 6) {
				Dbg.trc(Dbg.Grp.Terrain, 4, "fill");
				newBlk = topBlk.relative(0, +1, 0);
				cm.SetBlock(newBlk.coordinate, BlockProperties.BlockDirt);
				Dbg.msg(Dbg.Grp.Terrain, 4, "Filled a hole on top of a " + topBlk.properties.ToString() + " at " + x.ToString() + "," + z.ToString());
			}
		}
				
		//*****************************************************************************************
		/// replace blocktype old with new, dont do anything when no blocktype old at coordinate
		private void replaceBlock(int x, int y, int z, BlockProperties blockPropsOld, BlockProperties blockPropsNew) { // try {
			ChunkManager cm = AManager<ChunkManager>.getInstance();
			IBlock topBlk;
			
			Dbg.trc(Dbg.Grp.Terrain, 3);
			topBlk = getBlockOnTop(Coordinate.FromBlock(x, worldSize3i.y - 1, z));
			if(topBlk.properties.getID() == blockPropsOld.getID()) {
				cm.SetBlock(topBlk.coordinate, blockPropsNew);
				Dbg.msg(Dbg.Grp.Terrain, 4, "Replaced type " + blockPropsOld.ToString() + " with " + blockPropsNew.ToString() + " at " + x.ToString() + "," + z.ToString());
			}
		}  // catch (Exception e) {exceptionPrint(e);}}
		
		//*****************************************************************************************
		/// put a block on top of map at coordinate, ignores height y, replaces air
		private void putBlockOnTop(int x, int y, int z, BlockProperties blockPropsNew) { //try {
			ChunkManager cm = AManager<ChunkManager>.getInstance();
			IBlock topBlk;
			IBlock newBlk;
			
			Dbg.trc(Dbg.Grp.Terrain, 3);
			topBlk = getBlockOnTop(Coordinate.FromBlock(x, 0, z));
			newBlk = topBlk.relative(0, +1, 0);
			if(cm.isCoordInMap(newBlk.coordinate)) {
				cm.SetBlock(newBlk.coordinate, blockPropsNew);
			} else {
				Dbg.msg(Dbg.Grp.Terrain, 10, "Could not put Block on top at " + x.ToString() + "," + y.ToString() + "," + z.ToString());
			}
			Dbg.msg(Dbg.Grp.Terrain, 4, "Set type " + blockPropsNew.ToString() + " on top of a " + topBlk.properties.ToString() + " at " + x.ToString() + "," + z.ToString());
			
		} //catch (Exception e) {exceptionPrint(e);}}
		
		//*****************************************************************************************
		// get highest non-air block
		// ChunkManager.getBlockOnTop doesn't seem to work for me when searching from air/top down, not sure if I am using it wrong.. 
		private IBlock getBlockOnTop(Coordinate coord) { // try {
			ChunkManager cm = AManager<ChunkManager>.getInstance();
			IBlock topBlk;
			
			Dbg.trc(Dbg.Grp.Terrain, 3);
			topBlk = cm.GetBlock(coord);
			while(topBlk.properties.getID() == BlockProperties.BlockAir.getID()) {
				topBlk = topBlk.relative(0, -1, 0);
			}
			;
			
			return topBlk;
		}  // catch (Exception e) {exceptionPrint(e);}}
		
		
		
		//*****************************************************************************************
		//create random rain drops over the map
		
		IEnumerator createRainDrops(int xpos, int zpos, int xext, int zext, int type) {
			
			if(Configuration.getInstance().isEnabledShowRainBlocks.get() == 0) {
				yield return null;
			}
			
			Dbg.trc(Dbg.Grp.Rain, 5, "start");
			
			ChunkManager cm = AManager<ChunkManager>.getInstance();
			Rain rain = GetEntity<Rain>();			
			IBlock topBlk;
			IBlock newBlk;
			int dropRate;
			
			rain.startRain(type);
			
			switch(type) {
				case 1: dropRate = 0; break;
				case 2: dropRate = 40; break;
				case 3: dropRate = 80; break;
				default: dropRate = 40; break;
			}
			
			for(int x = xpos; x < xext; x++) {
				for(int z = zpos; z < zext; z++) {
					if(UnityEngine.Random.Range(0,200-dropRate) != 1) {
						continue;
					}
					
					Dbg.trc(Dbg.Grp.Rain, 3, "tryRandom");
					topBlk = getBlockOnTop(Coordinate.FromBlock(x, worldSize3i.y - 1, z));
					int height = UnityEngine.Random.Range(10, 20);
					if((topBlk.coordinate.absolute.y + height) >= (worldSize3i.y - 1)) {
						continue;
					}
					newBlk = topBlk.relative(0, height, 0);
					Dbg.msg(Dbg.Grp.Rain, 4, "New raindrop over" + topBlk.coordinate.ToString());
					rain.addRainDrop(newBlk.coordinate.world, topBlk.coordinate.world);
				}
				yield return null;
			}
		}

		//*****************************************************************************************
		//*****************************************************************************************
				
		
		//place markers on map as a debugging aid, not used atm
		private void placeMapMarkers() {
			for(int i = 0; i < 3; i++) {
				putBlockOnTop(0, 0, 0, BlockProperties.BlockIronOre);
			}
			for(int i = 0; i < 3; i++) {
				putBlockOnTop(worldSize3i.x, 0, 0, BlockProperties.BlockSilverOre);
			}
			for(int i = 0; i < 3; i++) {
				putBlockOnTop(0, 0, worldSize3i.z, BlockProperties.BlockGoldOre);
			}
			for(int i = 0; i < 3; i++) {
				putBlockOnTop(worldSize3i.x, 0, worldSize3i.z, BlockProperties.BlockMithrilOre);
			}
			//for (int i = 1; i < worldSize3i.x; i++)
			//	putBlockOnTop (i, 0, i, BlockProperties.BlockCopperOre.getID());
		}
		
	}
}

