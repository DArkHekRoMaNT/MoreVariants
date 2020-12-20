using System;
using System.Text;
using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace MoreVariantsFixes
{
    public class FixTypedContainer : EntityBehaviorPlaceBlock
    {
        ICoreAPI Api { get => player?.Api; }
        IWorldAccessor World { get => player?.World; }
        EntityPlayer player;
        public FixTypedContainer(Entity entity) : base(entity)
        {
            player = entity as EntityPlayer;
        }
        public override string PropertyName()
        {
            return "fixtypedcontainer";
        }
        public override void OnGameTick(float deltaTime)
        {
            base.OnGameTick(deltaTime);

            if (player.BlockSelection != null && false)
            {
                BlockPos pos = player.BlockSelection.Position;
                BlockEntityGenericTypedContainer be = World.BlockAccessor.GetBlockEntity(pos) as BlockEntityGenericTypedContainer;

                if (be?.Block?.Code?.Domain == "morevariants")
                {
                    if (be.Block.FirstCodePart() == "chest")
                    {
                        if (be.type != "normal" && be.type != "normal-generic")
                        {
                            BlockGenericTypedContainer newBlock = World.BlockAccessor.GetBlock(new AssetLocation("morevariants",
                                be.Block.FirstCodePart() + "-" + be.type + "-" + be.Block.LastCodePart()
                            )) as BlockGenericTypedContainer;

                            if (newBlock != null)
                            {
                                be.Block = newBlock;
                                be.type = "normal-generic";
                                World.BlockAccessor.SetBlock(newBlock.BlockId, pos);
                            }
                        }
                        else
                        {
                            be.type = "normal-generic";
                        }
                    }
                    else if (be.Block.FirstCodePart() == "labeledchest")
                    {
                        if (be.type != "normal" && be.type != "normal-labeled" && be.type != "normal-generic")
                        {
                            BlockLabeledChest newBlock = World.BlockAccessor.GetBlock(new AssetLocation("morevariants",
                                be.Block.FirstCodePart() + "-" + be.type.Split('-')[0] + "-" + be.Block.LastCodePart()
                            )) as BlockLabeledChest;

                            if (newBlock != null)
                            {
                                be.Block = newBlock;
                                World.BlockAccessor.SetBlock(newBlock.BlockId, pos);
                                be.type = "normal-labeled";
                            }
                        }
                        else
                        {

                            be.type = "normal-labeled";
                        }
                    }
                }



                /*World.Api.Logger.Warning("{0}: Obsolete {1} on {2} will be removed", Constants.MOD_ID, block.Code, pos);
                //try
                {
                    TreeAttribute tree = new TreeAttribute();
                    be.Inventory.ToTreeAttributes(tree);

                    AssetLocation code = new AssetLocation("morevariants",
                        block.FirstCodePart() + "-" + be.type.Split('-')[0] + "-" + block.LastCodePart()
                    );
                    Block newBlock = World.BlockAccessor.GetBlock(code);

                    World.BlockAccessor.SetBlock(newBlock.BlockId, pos);
                    be = World.BlockAccessor.GetBlockEntity(pos) as BlockEntityGenericTypedContainer;

                    be.Inventory.FromTreeAttributes(tree);
                    World.Api.Logger.Warning("{0}: Success", Constants.MOD_ID);
                }
                /*catch (Exception e)
                {
                    World.Api.Logger.Warning("{0}: Fail", Constants.MOD_ID);
                    World.Api.Logger.Error(e.Message);
                }*/
            }
        }
    }
    public class BEFixTypedChest : BlockEntityBehavior
    {
        public BEFixTypedChest(BlockEntity blockentity) : base(blockentity) { }
        public override void Initialize(ICoreAPI api, JsonObject properties)
        {
            base.Initialize(api, properties);
            Blockentity.RegisterGameTickListener(OnGameTick, 10000);
        }
        private void OnGameTick(float dt)
        {
            BlockEntityGenericTypedContainer be = Blockentity as BlockEntityGenericTypedContainer;

            if (be?.Block?.Code?.Domain == "morevariants" && be.type != "normal-labeled" && be.type != "normal-generic")
            {
                IBlockAccessor blockAccessor = Blockentity.Api.World.BlockAccessor;

                string newBEType;
                if (be.Block.FirstCodePart() == "chest")
                {
                    newBEType = "normal-generic";
                }
                else if (be.Block.FirstCodePart() == "labeledchest")
                {
                    newBEType = "normal-labeled";
                }
                else return;

                BlockGenericTypedContainer newBlock;
                if (be.type == "normal")
                {
                    newBlock = blockAccessor.GetBlock(be.Block.BlockId) as BlockGenericTypedContainer;
                }
                else
                {
                    newBlock = blockAccessor.GetBlock(new AssetLocation("morevariants",
                        be.Block.FirstCodePart() + "-" + be.type.Split('-')[0] + "-" + be.Block.LastCodePart()
                    )) as BlockGenericTypedContainer;
                }

                if (newBlock != null)
                {
                    TreeAttribute tree = new TreeAttribute();
                    be.Inventory.ToTreeAttributes(tree);
                    float angle = be.MeshAngle;

                    be.Block = newBlock;
                    blockAccessor.SetBlock(newBlock.BlockId, be.Pos);

                    be = blockAccessor.GetBlockEntity(be.Pos.Copy()) as BlockEntityGenericTypedContainer;
                    be.Inventory.FromTreeAttributes(tree);
                    be.type = newBEType;
                    be.MeshAngle = angle;
                }
            }
        }
    }
}
