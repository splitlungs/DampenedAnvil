﻿// using System;
// using System.Collections.Generic;
// using Vintagestory.API.Client;
// using Vintagestory.API.Common;
// using Vintagestory.API.MathTools;
// using Vintagestory.API.Util;
// using Vintagestory.GameContent;
// 
// namespace DampenedAnvil;
// 
// public class DampenedAnvilBlock : Block
// {
//     private WorldInteraction[] interactions;
//     
//     public override void OnLoaded(ICoreAPI api)
//     {
//         base.OnLoaded(api);
//         if (api.Side != EnumAppSide.Client)
//         {
//             return;
//         }
// 
//         _ = api;
//         Dictionary<string, MetalPropertyVariant> metalsByCode = new Dictionary<string, MetalPropertyVariant>();
//         MetalProperty metalProperty = api.Assets.TryGet("worldproperties/block/metal.json").ToObject<MetalProperty>();
//         for (int i = 0; i < metalProperty.Variants.Length; i++)
//         {
//             metalsByCode[metalProperty.Variants[i].Code.Path] = metalProperty.Variants[i];
//         }
// 
//         string metalType = LastCodePart();
//         int ownMetalTier = 0;
//         if (metalsByCode.ContainsKey(metalType))
//         {
//             ownMetalTier = metalsByCode[metalType].Tier;
//         }
// 
//         interactions = ObjectCacheUtil.GetOrCreate(api, "anvilBlockInteractions" + ownMetalTier, delegate
//         {
//             List<ItemStack> list = new List<ItemStack>();
//             List<ItemStack> list2 = new List<ItemStack>();
//             bool flag = metalsByCode.ContainsKey(metalType) && metalsByCode[metalType].Tier <= ownMetalTier + 1;
//             foreach (Item item in api.World.Items)
//             {
//                 if (!(item.Code == null))
//                 {
//                     if (item is ItemIngot && flag)
//                     {
//                         list.Add(new ItemStack(item));
//                     }
// 
//                     if (item is ItemHammer)
//                     {
//                         list2.Add(new ItemStack(item));
//                     }
//                 }
//             }
// 
//             return new WorldInteraction[6]
//             {
//                 new WorldInteraction
//                 {
//                     ActionLangCode = "blockhelp-anvil-takeworkable",
//                     HotKeyCode = null,
//                     MouseButton = EnumMouseButton.Right,
//                     ShouldApply = (WorldInteraction wi, BlockSelection bs, EntitySelection es) => (api.World.BlockAccessor.GetBlockEntity(bs.Position) as BlockEntityAnvil)?.WorkItemStack != null
//                 },
//                 new WorldInteraction
//                 {
//                     ActionLangCode = "blockhelp-anvil-placeworkable",
//                     HotKeyCode = "shift",
//                     MouseButton = EnumMouseButton.Right,
//                     Itemstacks = list.ToArray(),
//                     GetMatchingStacks = (WorldInteraction wi, BlockSelection bs, EntitySelection es) => ((api.World.BlockAccessor.GetBlockEntity(bs.Position) as BlockEntityAnvil)?.WorkItemStack != null) ? null : wi.Itemstacks
//                 },
//                 new WorldInteraction
//                 {
//                     ActionLangCode = "blockhelp-anvil-smith",
//                     MouseButton = EnumMouseButton.Left,
//                     Itemstacks = list2.ToArray(),
//                     GetMatchingStacks = (WorldInteraction wi, BlockSelection bs, EntitySelection es) => ((api.World.BlockAccessor.GetBlockEntity(bs.Position) as BlockEntityAnvil)?.WorkItemStack != null) ? wi.Itemstacks : null
//                 },
//                 new WorldInteraction
//                 {
//                     ActionLangCode = "blockhelp-anvil-rotateworkitem",
//                     MouseButton = EnumMouseButton.Right,
//                     Itemstacks = list2.ToArray(),
//                     GetMatchingStacks = (WorldInteraction wi, BlockSelection bs, EntitySelection es) => ((api.World.BlockAccessor.GetBlockEntity(bs.Position) as BlockEntityAnvil)?.WorkItemStack != null) ? wi.Itemstacks : null
//                 },
//                 new WorldInteraction
//                 {
//                     ActionLangCode = "blockhelp-selecttoolmode",
//                     HotKeyCode = "toolmodeselect",
//                     MouseButton = EnumMouseButton.None,
//                     Itemstacks = list2.ToArray(),
//                     GetMatchingStacks = (WorldInteraction wi, BlockSelection bs, EntitySelection es) => ((api.World.BlockAccessor.GetBlockEntity(bs.Position) as BlockEntityAnvil)?.WorkItemStack != null) ? wi.Itemstacks : null
//                 },
//                 new WorldInteraction
//                 {
//                     ActionLangCode = "blockhelp-anvil-addvoxels",
//                     HotKeyCode = "shift",
//                     MouseButton = EnumMouseButton.Right,
//                     Itemstacks = list.ToArray(),
//                     GetMatchingStacks = delegate(WorldInteraction wi, BlockSelection bs, EntitySelection es)
//                     {
//                         BlockEntityAnvil blockEntityAnvil = api.World.BlockAccessor.GetBlockEntity(bs.Position) as BlockEntityAnvil;
//                         return (blockEntityAnvil?.WorkItemStack != null) ? new ItemStack[1] { (blockEntityAnvil.WorkItemStack.Collectible as IAnvilWorkable).GetBaseMaterial(blockEntityAnvil.WorkItemStack) } : null;
//                     }
//                 }
//             };
//         });
//     }
// 
//     public override void OnDecalTesselation(IWorldAccessor world, MeshData decalMesh, BlockPos pos)
//     {
//         base.OnDecalTesselation(world, decalMesh, pos);
//         if (world.BlockAccessor.GetBlockEntity(pos) is DampenedAnvilBE blockEntityAnvil)
//         {
//             decalMesh.Rotate(new Vec3f(0.5f, 0.5f, 0.5f), 0f, blockEntityAnvil.MeshAngle, 0f);
//         }
//     }
// 
//     public override Cuboidf[] GetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos)
//     {
//         if (blockAccessor.GetBlockEntity(pos) is DampenedAnvilBE blockEntityAnvil)
//         {
//             Cuboidf[] selectionBoxes = blockEntityAnvil.GetSelectionBoxes(blockAccessor, pos);
//             float num = Math.Abs(blockEntityAnvil.MeshAngle * (180f / MathF.PI));
//             selectionBoxes[0] = ((num == 0f || num == 180f) ? SelectionBoxes[0] : SelectionBoxes[1]);
//             return selectionBoxes;
//         }
// 
//         return base.GetSelectionBoxes(blockAccessor, pos);
//     }
// 
//     public override Cuboidf[] GetCollisionBoxes(IBlockAccessor blockAccessor, BlockPos pos)
//     {
//         return GetSelectionBoxes(blockAccessor, pos);
//     }
// 
//     public override Cuboidf[] GetParticleCollisionBoxes(IBlockAccessor blockAccessor, BlockPos pos)
//     {
//         return GetSelectionBoxes(blockAccessor, pos);
//     }
// 
//     public override bool DoParticalSelection(IWorldAccessor world, BlockPos pos)
//     {
//         return true;
//     }
// 
//     public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
//     {
//         if (world.BlockAccessor.GetBlockEntity(blockSel.Position) is DampenedAnvilBE blockEntityAnvil)
//         {
//             if (blockEntityAnvil.OnPlayerInteract(world, byPlayer, blockSel))
//             {
//                 return true;
//             }
// 
//             return false;
//         }
// 
//         return base.OnBlockInteractStart(world, byPlayer, blockSel);
//     }
// 
//     public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer)
//     {
//         return interactions.Append(base.GetPlacedBlockInteractionHelp(world, selection, forPlayer));
//     }
// 
//     public override bool DoPlaceBlock(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ItemStack byItemStack)
//     {
//         bool num = base.DoPlaceBlock(world, byPlayer, blockSel, byItemStack);
//         if (num && world.BlockAccessor.GetBlockEntity(blockSel.Position) is DampenedAnvilBE blockEntityAnvil)
//         {
//             BlockPos blockPos = (blockSel.DidOffset ? blockSel.Position.AddCopy(blockSel.Face.Opposite) : blockSel.Position);
//             double y = byPlayer.Entity.Pos.X - ((double)blockPos.X + blockSel.HitPosition.X);
//             double x = byPlayer.Entity.Pos.Z - ((double)blockPos.Z + blockSel.HitPosition.Z);
//             float num2 = (float)Math.Atan2(y, x);
//             float num3 = MathF.PI / 8f;
//             float meshAngle = (float)(int)Math.Round(num2 / num3) * num3;
//             blockEntityAnvil.MeshAngle = meshAngle;
//         }
// 
//         return num;
//     }
// }