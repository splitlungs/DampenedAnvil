// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using Vintagestory.API.Common;
// using DampenedAnvil;
// using Vintagestory.GameContent.Mechanics;
// 
// namespace Vintagestory.GameContent;
// 
// public static class IAnvilWorkable_Additions
// {
//     public static ItemStack TryPlaceOn(this IAnvilWorkable anvil, ItemStack stack, DampenedAnvilBE beAnvil)
//     {
//         BlockEntityAnvil beA = beAnvil as BlockEntityAnvil;
//         return anvil.TryPlaceOn(stack, beA);
//     }
//     
//     public static EnumHelveWorkableMode GetHelveWorkableMode(this IAnvilWorkable anvil, ItemStack stack, DampenedAnvilBE beAnvil)
//     {
//         BlockEntityAnvil beA = beAnvil as BlockEntityAnvil;
//         return anvil.GetHelveWorkableMode(stack, beA);
//     }
// }
