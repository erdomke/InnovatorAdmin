using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace InnovatorAdmin.Editor
{
  public static class Icons
  {
    public static IEnumerable<IconInfo> All { get; private set; }

    public static IconInfo AbstractClass16 { get; } = new IconInfo("abstract-class-16", @"<DrawingGroup xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
  <DrawingGroup.Children>
    <GeometryDrawing Brush='#00FFFFFF' Geometry='F1M16,16L0,16 0,0 16,0z' />
    <GeometryDrawing Brush='#FFF6F6F6' Geometry='F1M0,16L16,16 16,0 0,0z M6.416,3L13,3 13,7.685 10.209,4.901 9.265,5.849z M3,3L5.588,3 3,5.588z M11.383,9.331L13,7.714 13,10.945z M3,8.413L4.819,10.223 5,10.041 5,12 7.195,12 8.196,13 3,13z M13,10.982L13,13 10.981,13z' />
    <GeometryDrawing Brush='#FF424242' Geometry='F1M14,14L13,14 13,15 15,15 15,13 14,13z M10,2L12,2 12,1 10,1z M10,15L12,15 12,14 10,14z M14,9L15,9 15,7 14,7z M13,1L13,2 14,2 14,3 15,3 15,1z M14,12L15,12 15,10 14,10z M7,15L9,15 9,14 7,14z M14,6L15,6 15,4 14,4z M6,1L4,1 4,2 6,2z M2,7L1,7 1,9 2,9z M2,4L1,4 1,6 2,6z M4,15L6,15 6,14 4,14z M9,1L7,1 7,2 9,2z M2,2L3,2 3,1 1,1 1,3 2,3z M2,10L1,10 1,12 2,12z M2,13L1,13 1,15 3,15 3,14 2,14z M11.604,10.964L9.589,12.977 8.201,11.592 8.796,11 6,11 6,7.995 6,7.698 6,7.637 4.818,8.812 3,7.001 6.001,4 7.813,5.813 6.628,7 6.689,7 6.986,7 9.529,7 10.211,6.316 11.6,7.7 9.585,9.715 8.197,8.33 8.529,8 7,8 7,10 9.795,10 10.215,9.578z' />
  </DrawingGroup.Children>
</DrawingGroup>");
    public static IconInfo Assembly16 { get; } = new IconInfo("assembly-16", @"<DrawingGroup xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
  <DrawingGroup.Children>
    <GeometryDrawing Brush='#00FFFFFF' Geometry='F1M16,16L0,16 0,0 16,0z' />
    <GeometryDrawing Brush='#FFF6F6F6' Geometry='F1M3.9996,-0.000199999999999534L3.9996,1.9998 1.9996,1.9998 1.9996,4.0008 -0.000399999999999956,4.0008 -0.000399999999999956,15.9998 11.9996,15.9998 11.9996,13.9998 14.0006,13.9998 14.0006,12.0008 16.0006,12.0008 16.0006,-0.000199999999999534z' />
    <GeometryDrawing Brush='#FF424242' Geometry='F1M14,10L13,10 13,3 6,3 6,2 14,2z M12,12L11,12 11,5 4,5 4,4 12,4z M10,14L2,14 2,6 10,6z M5,1L5,3 3,3 3,5 1,5 1,15 11,15 11,13 13,13 13,11 15,11 15,1z' />
    <GeometryDrawing Brush='#FFF0EFF1' Geometry='F1M14,10L13,10 13,3 6,3 6,2 14,2z M12,12L11,12 11,5 4,5 4,4 12,4z M10,14L2,14 2,6 10,6z' />
  </DrawingGroup.Children>
</DrawingGroup>");
    public static IconInfo Attribute16 { get; } = new IconInfo("attribute-16", @"<DrawingGroup xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
  <DrawingGroup.Children>
    <GeometryDrawing Brush='#00FFFFFF' Geometry='F1M16,16L0,16 0,0 16,0z' />
    <GeometryDrawing Brush='#FFF6F6F6' Geometry='F1M7.9746,16C6.8356,16 5.7726,15.82 4.8146,15.464 3.8376,15.102 2.9826,14.574 2.2716,13.9 1.5466,13.211 0.981599999999999,12.383 0.5916,11.44 0.199599999999999,10.502 -0.000399999999999956,9.441 -0.000399999999999956,8.292 -0.000399999999999956,7.536 0.0876000000000001,6.802 0.260599999999999,6.11 0.4306,5.419 0.6846,4.755 1.0136,4.14 1.3396,3.527 1.7516,2.956 2.2376,2.446 2.7226,1.937 3.2776,1.495 3.8886,1.134 4.4936,0.776 5.1696,0.494 5.8976,0.295 6.6206,0.0990000000000002 7.4016,0 8.2216,0 9.2556,0 10.2496,0.159000000000001 11.1726,0.474 12.1056,0.789999999999999 12.9396,1.255 13.6486,1.856 14.3686,2.467 14.9476,3.22 15.3666,4.094 15.7866,4.991 15.9996,5.989 15.9996,7.068 15.9996,7.967 15.8706,8.783 15.6176,9.492 15.3616,10.195 15.0176,10.803 14.5936,11.304 14.1386,11.824 13.6276,12.215 13.0646,12.476 12.9346,12.537 12.8026,12.592 12.6686,12.639L12.6686,15.04 12.0796,15.305C11.8956,15.389 11.6656,15.471 11.3966,15.559 11.1326,15.645 10.8246,15.721 10.4736,15.787 10.1346,15.848 9.7636,15.901 9.3506,15.941 8.9566,15.98 8.5046,16 7.9746,16' />
    <GeometryDrawing Brush='#FF424242' Geometry='F1M9.3174,8.4902C9.1174,9.5042 8.4164,10.9022 7.1374,10.6002 5.8494,10.2952 5.9124,8.5472 6.1334,7.5572 6.3614,6.5412 7.1004,5.2132 8.3494,5.4652 9.7014,5.7372 9.5014,7.4922 9.3174,8.4902 M13.0024,2.6192C10.7694,0.7252 7.2234,0.4902 4.6684,1.8432 2.0884,3.2092 0.798399999999999,6.1082 1.0254,8.9562 1.2644,11.9602 3.3944,14.2932 6.3504,14.8542 7.7154,15.1132 9.2264,15.0552 10.5784,14.7452 10.8104,14.6922 11.3784,14.6232 11.5364,14.4462 11.7194,14.2422 11.5854,14.4102 11.6694,14.1062 11.7514,13.8062 11.6694,13.3762 11.6694,13.0652 9.2874,14.1082 5.9544,14.1812 4.0124,12.2152 2.0304,10.2072 2.1094,6.6412 3.7344,4.4512 5.3774,2.2372 8.6404,1.5892 11.0604,2.8882 13.3394,4.1122 14.3364,7.6682 12.8174,9.8612 12.4984,10.3222 11.7884,10.9112 11.1984,10.5132 10.7294,10.1972 10.7734,9.3722 10.7904,8.8782 10.8394,7.3662 11.0984,5.8642 11.1654,4.3542 10.8624,4.3542 10.0464,4.1952 9.8154,4.3542 9.6384,4.4762 9.6764,5.0232 9.6524,5.2862 8.9984,4.0152 7.4694,3.9662 6.3034,4.7012 4.9904,5.5282 4.4424,7.1712 4.4494,8.6582 4.4554,9.9702 4.9824,11.3972 6.3664,11.7722 7.9664,12.2052 8.7754,11.1592 9.4424,9.9732 9.4424,11.1932 10.2714,11.9292 11.4724,11.8662 12.8054,11.7972 13.8674,10.8752 14.4374,9.7182 15.5924,7.3782 14.9934,4.3062 13.0024,2.6192' />
  </DrawingGroup.Children>
</DrawingGroup>");
    public static IconInfo Class16 { get; } = new IconInfo("class-16", @"<DrawingGroup xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
  <DrawingGroup.Children>
    <GeometryDrawing Brush='#00FFFFFF' Geometry='F1M16,16L0,16 0,0 16,0z' />
    <GeometryDrawing Brush='#FFF6F6F6' Geometry='F1M5.5863,-0.000199999999999534L0.000299999999999301,5.5858 0.000299999999999301,6.4138 3.9993,10.4138 6.4143,7.9998 7.0003,7.9998 7.0003,9.9998 7.0003,13.0008 8.5863,13.0008 11.5853,15.9998 12.4133,15.9998 16.0003,12.4148 16.0003,11.5858 13.9143,9.4998 16.0003,7.4138 16.0003,6.5858 12.9993,3.5868 11.5853,4.9998 10.0003,4.9998 9.4143,4.9998 10.4143,4.0008 6.4143,-0.000199999999999534z' />
    <GeometryDrawing Brush='#FFC17C1A' Geometry='F1M13,10L15,12 12,15 10,13 11,12 8,12 8,7 6,7 4,9 1,6 6,1 9,4 7,6 12,6 13,5 15,7 12,10 10,8 11,7 9,7 9,11 11.999,11.002z' />
  </DrawingGroup.Children>
</DrawingGroup>");
    public static IconInfo ClassCollection16 { get; } = new IconInfo("class-collection-16", @"<DrawingGroup xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
  <DrawingGroup.Children>
    <GeometryDrawing Brush='#00FFFFFF' Geometry='F1M16,16L0,16 0,0 16,0z' />
    <GeometryDrawing Brush='#FFF6F6F6' Geometry='F1M9.0018,-0.000199999999999534L5.7858,3.2158 4.9888,2.4178 -0.00020000000000131,7.4058 -0.00020000000000131,8.2348 3.3928,11.6278 3.9878,11.0328 3.9878,13.6078 5.3718,13.6078 7.7638,15.9998 8.5928,15.9998 11.8098,12.7848 12.6058,13.5818 15.9998,10.1898 15.9998,9.3618 14.4198,7.7808 15.9998,6.1998 15.9998,5.3718 13.4048,2.7768 13.0058,3.1748 9.8298,-0.000199999999999534z' />
    <GeometryDrawing Brush='#FF378A33' Geometry='F1M8.1777,11.8105L5.7857,11.8105 5.7857,8.6185 7.3817,8.6185 6.5837,9.4155 8.1787,11.0115 10.5727,8.6185 8.9767,7.0225 8.1787,7.8205 5.7857,7.8205 7.3817,6.2255 4.9887,3.8315 0.9997,7.8205 3.3927,10.2135 4.9887,8.6185 4.9887,12.6075 7.3817,12.6075 6.5837,13.4055 8.1787,14.9995 10.5727,12.6075 8.9767,11.0115z M12.6057,8.9775L11.6267,8.9775 10.8297,9.7755 11.8097,9.7755 11.0117,10.5725 12.6067,12.1675 14.9997,9.7755 13.4047,8.1795z M6.4927,3.9225L9.4157,0.999500000000001 11.8097,3.3935 10.2137,4.9885 12.6067,4.9885 13.4047,4.1905 14.9997,5.7865 12.6067,8.1795 11.0117,6.5835 11.8097,5.7865 10.2137,5.7865 10.2137,6.8465 9.4157,6.0485 9.4157,5.7865 9.2847,5.9165 8.9767,5.6085 8.5777,6.0075z' />
  </DrawingGroup.Children>
</DrawingGroup>");
    public static IconInfo ClassMoved16 { get; } = new IconInfo("class-moved-16", @"<DrawingGroup xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
  <DrawingGroup.Children>
    <GeometryDrawing Brush='#00FFFFFF' Geometry='F1M16,16L0,16 0,0 16,0z' />
    <GeometryDrawing Brush='#FFF6F6F6' Geometry='F1M3.5856,-0.000199999999999534L-0.000399999999999956,3.5868 -0.000399999999999956,5.4138 2.5006,7.9148 3.9996,6.4138 3.9996,9.9998 6.5866,9.9998 7.0446,10.4578 7.0006,10.4578 7.0006,13.4588 10.7566,13.4588 9.6106,14.6078 11.0046,15.9998 12.4586,15.9998 16.0006,12.4708 16.0006,11.4768 10.4136,5.9998 11.9146,4.4998 8.9996,1.5858 7.5866,3.0008 7.4146,3.0008 4.4146,-0.000199999999999534z' />
    <GeometryDrawing Brush='#FFC27D1A' Geometry='F1M0.5004,4.4998L2.5004,6.4998 3.9994,4.9998 5.0004,4.9998 5.0004,8.9998 8.0004,8.9998 7.4994,9.4998 8.4584,10.4578 10.7384,10.4578 9.6114,9.3278 10.9854,7.9518 10.8264,7.8258 9.9994,6.9998 9.0004,7.9998 6.0004,7.9998 6.0004,4.9998 7.0004,4.9998 6.4994,5.4998 8.0004,6.9998 10.5004,4.4998 9.0004,3.0008 8.0004,4.0008 6.0004,4.0008 5.0004,4.0008 6.0004,3.0008 3.9994,0.9998z' />
    <GeometryDrawing Brush='#FF424242' Geometry='F1M11.0241,9.3283L13.1461,11.4573 8.0001,11.4573 8.0001,12.4573 13.1641,12.4573 11.0241,14.6073 11.7311,15.3133 15.0831,11.9733 11.7311,8.6213z' />
  </DrawingGroup.Children>
</DrawingGroup>");
    public static IconInfo DatabaseMethod16 { get; } = new IconInfo("database-method-16", @"<DrawingGroup xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
  <DrawingGroup.Children>
    <GeometryDrawing Brush='#00FFFFFF' Geometry='F1M16,16L0,16 0,0 16,0z' />
    <GeometryDrawing Brush='#FFF6F6F6' Geometry='F1M16,3L16,9C16,10.71 13.851,12 11,12 10.664,12 10.325,11.978 9.937,11.929L9.755,11.9 9.755,13.381 6.182,16 4.253,16 0.167,13.426 0.167,8.613 0.157,8.607 0.167,8.602 0.167,7.856 5.051,5.387 6,5.887 6,3C6,1.29 8.149,0 11,0 13.851,0 16,1.29 16,3' />
    <GeometryDrawing Brush='#FF424242' Geometry='F1M1.1673,8.4715L1.1673,12.8745 5.2483,15.4445 8.7553,12.8745 8.7553,8.4715 5.0413,6.5125z M2.2643,8.6585L4.9963,7.2715 7.9083,8.7075 5.2003,10.3325z M1.8613,12.4555L1.8613,9.3055 4.9203,11.0145 4.9203,14.4505z M5.6903,11.0335L8.1153,9.5595 8.1153,12.5605 5.6903,14.3085z' />
    <GeometryDrawing Brush='#FFF0EFF1' Geometry='F1M11,1.6748C9.001,1.6748 7.675,2.4728 7.675,2.9998 7.675,3.5278 9.001,4.3248 11,4.3248 12.999,4.3248 14.325,3.5278 14.325,2.9998 14.325,2.4728 12.999,1.6748 11,1.6748 M8.115,9.5598L5.689,11.0338 5.689,14.3088 8.115,12.5608z M4.92,11.0148L1.861,9.3058 1.861,12.4558 4.92,14.4498z M7.909,8.7068L5.2,10.3328 2.265,8.6588 4.996,7.2718z' />
    <GeometryDrawing Brush='#FF1BA1E2' Geometry='F1M11,4.3252C9.001,4.3252 7.675,3.5272 7.675,3.0002 7.675,2.4722 9.001,1.6752 11,1.6752 12.999,1.6752 14.325,2.4722 14.325,3.0002 14.325,3.5272 12.999,4.3252 11,4.3252 M11,1.0002C8.791,1.0002,7,1.8952,7,3.0002L7,6.4152 9.755,7.8682 9.755,10.8862C9.836,10.9002,9.913,10.9182,9.996,10.9282L9.996,10.9292C9.997,10.9292 9.997,10.9292 9.997,10.9292 9.998,10.9292 9.999,10.9292 10,10.9292L10,10.9292C10.321,10.9712 10.653,11.0002 11,11.0002 13.209,11.0002 15,10.1042 15,9.0002L15,3.0002C15,1.8952,13.209,1.0002,11,1.0002' />
  </DrawingGroup.Children>
</DrawingGroup>");
    public static IconInfo EnumValue16 { get; } = new IconInfo("enum-value-16", @"<DrawingGroup xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
  <DrawingGroup.Children>
    <GeometryDrawing Brush='#00FFFFFF' Geometry='F1M16,16L0,16 0,0 16,0z' />
    <GeometryDrawing Brush='#FFF6F6F6' Geometry='F1M7.5852,0.999700000000001L6.0002,2.5867 6.0002,5.9997 0.000200000000000422,5.9997 0.000200000000000422,14.9997 10.0002,14.9997 10.0002,9.9997 14.4142,9.9997 16.0002,8.4147 16.0002,2.5867 14.4142,0.999700000000001z' />
    <GeometryDrawing Brush='#FFEFEFF0' Geometry='F1M7,11L3,11 3,10 7,10z M2,13L8,13 8,8 2,8z M13,5L9,5 9,4 13,4z M8,3L8,6 9,6 10,6 13,6 13,7 10,7 10,8 14,8 14,6 14,3z' />
    <GeometryDrawing Brush='#FF00529C' Geometry='F1M8,8L2,8 2,13 8,13z M9,14L1,14 1,7 9,7z M7,10L3,10 3,11 7,11z M14,2L8,2 7,3 7,6 8,6 8,3 14,3 14,6 14,8 10,8 10,9 14,9 15,8 15,3z M9,5L13,5 13,4 9,4z M10,6L13,6 13,7 10,7z' />
  </DrawingGroup.Children>
</DrawingGroup>");
    public static IconInfo Event16 { get; } = new IconInfo("event-16", @"<DrawingGroup xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
  <DrawingGroup.Children>
    <GeometryDrawing Brush='#00FFFFFF' Geometry='F1M16,16L0,16 0,0 16,0z' />
    <GeometryDrawing Brush='#FFF6F6F6' Geometry='F1M6.3819,-0.000199999999999534L1.9999,8.7638 1.9999,9.9998 5.3709,9.9998 2.9999,14.7648 2.9999,15.9998 5.4149,15.9998 13.9999,7.4138 13.9999,5.9998 9.4139,5.9998 13.9999,1.4138 13.9999,-0.000199999999999534z' />
    <GeometryDrawing Brush='#FFC17C1A' Geometry='F1M7,7L13,7 5,15 4,15 6.985,9 3,9 7,1 13,1z' />
  </DrawingGroup.Children>
</DrawingGroup>");
    public static IconInfo Field16 { get; } = new IconInfo("field-16", @"<DrawingGroup xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
  <DrawingGroup.Children>
    <GeometryDrawing Brush='#00FFFFFF' Geometry='F1M16,16L0,16 0,0 16,0z' />
    <GeometryDrawing Brush='#FFF6F6F6' Geometry='F1M9,-0.000199999999999534L0,4.4998 0,10.7358 7,14.2358 16,9.7358 16,3.4998z' />
    <GeometryDrawing Brush='#FF00529C' Geometry='F1M7,6.8818L3.236,4.9998 9,2.1178 12.764,3.9998z M9,0.9998L1,4.9998 1,9.9998 7,12.9998 15,8.9998 15,3.9998z' />
    <GeometryDrawing Brush='#FFEFEFF0' Geometry='F1M9,2.1182L12.764,4.0002 7,6.8822 3.236,5.0002z' />
  </DrawingGroup.Children>
</DrawingGroup>");
    public static IconInfo Folder16 { get; } = new IconInfo("folder-16", @"<DrawingGroup xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
  <DrawingGroup.Children>
    <GeometryDrawing Brush='#00FFFFFF' Geometry='F1M0,0L16,0 16,16 0,16z' />
    <GeometryDrawing Brush='#FFF6F6F6' Geometry='F1M1.5,1L9.61,1 10.61,3 13.496,3C14.323,3,14.996,3.673,14.996,4.5L14.996,12.5C14.996,13.327,14.323,14,13.496,14L1.5,14C0.673,14,0,13.327,0,12.5L0,2.5C0,1.673,0.673,1,1.5,1' />
    <GeometryDrawing Brush='#FFEFEFF0' Geometry='F1M1.9998,3.0004L1.9998,4.0004 8.8738,4.0004 8.3738,3.0004z' />
    <GeometryDrawing Brush='#FFDBB679' Geometry='F1M2,3L8.374,3 8.874,4 2,4z M13.496,4L10,4 9.992,4 8.992,2 1.5,2C1.225,2,1,2.224,1,2.5L1,12.5C1,12.776,1.225,13,1.5,13L13.496,13C13.773,13,13.996,12.776,13.996,12.5L13.996,4.5C13.996,4.224,13.773,4,13.496,4' />
  </DrawingGroup.Children>
</DrawingGroup>");
    public static IconInfo FolderSpecial16 { get; } = new IconInfo("folder-special-16", @"<DrawingGroup xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
  <DrawingGroup.Children>
    <GeometryDrawing Brush='#00FFFFFF' Geometry='F1M16,16L0,16 0,0 16,0z' />
    <GeometryDrawing Brush='#FFF6F6F6' Geometry='F1M14.9961,3.5L14.9961,11.5C14.9961,12.327,14.3231,13,13.4961,13L8.7241,13C8.1021,14.742 6.4531,16 4.5001,16 2.1221,16 0.1891,14.141 0.0301000000000009,11.802 0.0111000000000008,11.704 9.99999999997669E-05,11.604 9.99999999997669E-05,11.5L9.99999999997669E-05,1.5C9.99999999997669E-05,0.673,0.6731,0,1.5001,0L9.6101,0 10.6101,2 13.4961,2C14.3231,2,14.9961,2.673,14.9961,3.5' />
    <GeometryDrawing Brush='#FFDCB67A' Geometry='F1M2.0039,3L2.0039,2 8.3779,2 8.8779,3z M13.4999,3L10.0039,3 9.9959,3 8.9959,1 1.5039,1C1.2289,1,1.0039,1.224,1.0039,1.5L1.0039,8.702C1.8289,7.673 3.0809,7 4.4999,7 6.9819,7 8.9999,9.019 8.9999,11.5 8.9999,11.671 8.9679,11.834 8.9489,12L13.4999,12C13.7769,12,13.9999,11.776,13.9999,11.5L13.9999,3.5C13.9999,3.224,13.7769,3,13.4999,3' />
    <GeometryDrawing Brush='#FF424242' Geometry='F1M6,11.5C6,12.328 5.329,13 4.5,13 3.671,13 3,12.328 3,11.5 3,10.672 3.671,10 4.5,10 5.329,10 6,10.672 6,11.5 M7,11.5C7,12.881 5.881,14 4.5,14 3.119,14 2,12.881 2,11.5 2,10.119 3.119,9 4.5,9 5.881,9 7,10.119 7,11.5 M4.5,8C2.567,8 1,9.567 1,11.5 1,13.433 2.567,15 4.5,15 6.433,15 8,13.433 8,11.5 8,9.567 6.433,8 4.5,8' />
    <GeometryDrawing Brush='#FFF0EFF1' Geometry='F1M1.9998,1.9996L1.9998,3.0006 8.8738,3.0006 8.3738,1.9996z' />
  </DrawingGroup.Children>
</DrawingGroup>");
    public static IconInfo Method16 { get; } = new IconInfo("method-16", @"<DrawingGroup xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
  <DrawingGroup.Children>
    <GeometryDrawing Brush='#00FFFFFF' Geometry='F1M16,16L0,16 0,0 16,0z' />
    <GeometryDrawing Brush='#FFF6F6F6' Geometry='F1M7.5951,-0.000199999999999534L1.0001,3.3268 1.0001,11.5818 8.0701,15.9998 8.9751,15.9998 15.0001,11.7518 15.0001,3.3488 8.7131,-0.000199999999999534z' />
    <GeometryDrawing Brush='#FFEFEFF0' Geometry='F1M9,13.5361L9,7.8781 13,5.3981 13,10.7161z M3,5.1021L8,7.8941 8,13.5981 3,10.4731z M12.715,4.3981L8.487,7.0201 3.565,4.2721 8.144,1.9631z' />
    <GeometryDrawing Brush='#FF642D90' Geometry='F1M1.9998,3.9427L1.9998,11.0277 8.5168,15.0997 14.0008,11.2337 14.0008,3.9497 8.1558,0.8367z M3.5658,4.2717L8.1428,1.9627 12.7148,4.3977 8.4868,7.0197z M2.9998,10.4727L2.9998,5.1017 7.9998,7.8937 7.9998,13.5977z M8.9998,7.8787L12.9998,5.3977 12.9998,10.7157 8.9998,13.5357z' />
  </DrawingGroup.Children>
</DrawingGroup>");
    public static IconInfo MethodFriend16 { get; } = new IconInfo("method-friend-16", @"<DrawingGroup xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
  <DrawingGroup.Children>
    <GeometryDrawing Brush='#00FFFFFF' Geometry='F1M16,16L0,16 0,0 16,0z' />
    <GeometryDrawing Brush='#FFF6F6F6' Geometry='F1M0.167,8.4258L0.167,2.8568 5.051,0.386799999999999 9.755,2.8678 9.755,8.3808 5.291,11.6538z' />
    <GeometryDrawing Brush='#FFF6F6F6' Geometry='F1M13.5713,9C13.2953,9 12.8943,9.055 12.5003,9.305 12.1053,9.055 11.7053,9 11.4283,9 10.1123,9 9.0003,10.137 9.0003,11.482 9.0003,12.076 9.1963,12.67 9.5603,13.139 9.8533,13.493 11.8063,15.668 11.8063,15.668L12.1033,16 13.0123,16 13.3123,15.648C13.3123,15.648 15.1483,13.489 15.4603,13.113 15.8023,12.671 16.0003,12.076 16.0003,11.482 16.0003,10.137 14.8873,9 13.5713,9' />
    <GeometryDrawing Brush='#FF642C90' Geometry='F1M8.1904,7.5986L5.6144,9.4546 5.6144,5.9926 8.1904,4.4256z M4.9704,9.5616L4.9704,9.5776 4.9614,9.5696 4.9534,9.5776 4.9534,9.5616 1.7864,7.4966 1.7864,4.1776 4.9954,5.9706 4.9954,9.5436z M4.9954,2.1876L8.0654,3.7016 5.2014,5.4196 2.1064,3.6546z M8.7544,7.8736L8.7544,3.4706 5.0414,1.5116 1.1674,3.4706 1.1674,7.8736 5.2484,10.4446z' />
    <GeometryDrawing Brush='#FFF0EFF1' Geometry='F1M4.9951,2.1875L8.0651,3.7015 5.2011,5.4195 2.1061,3.6545z'>
      <GeometryDrawing.Pen>
        <Pen Brush='#FF642C90' Thickness='0.15000006556510925' DashCap='Flat' />
      </GeometryDrawing.Pen>
    </GeometryDrawing>
    <GeometryDrawing Brush='#FFF0EFF1' Geometry='F1M4.9707,9.5615L4.9707,9.5775 4.9607,9.5695 4.9527,9.5775 4.9527,9.5615 1.7857,7.4975 1.7857,4.1775 4.9947,5.9705 4.9947,9.5435z'>
      <GeometryDrawing.Pen>
        <Pen Brush='#FF642C90' Thickness='0.15000006556510925' DashCap='Flat' />
      </GeometryDrawing.Pen>
    </GeometryDrawing>
    <GeometryDrawing Brush='#FFF0EFF1' Geometry='F1M8.1904,7.5986L5.6144,9.4546 5.6144,5.9926 8.1904,4.4256z'>
      <GeometryDrawing.Pen>
        <Pen Brush='#FF642C90' Thickness='0.15000006556510925' DashCap='Flat' />
      </GeometryDrawing.Pen>
    </GeometryDrawing>
    <GeometryDrawing Brush='#FF414141' Geometry='F1M13.5713,10C13.0443,10 12.7113,10.747 12.5003,11.25 12.2893,10.747 11.9553,10 11.4283,10 10.7013,10 10.0003,10.664 10.0003,11.482 10.0003,11.857 10.1273,12.238 10.3293,12.5 10.6193,12.85 12.5503,15 12.5503,15 12.5503,15 14.3803,12.85 14.6703,12.5 14.8733,12.238 15.0003,11.857 15.0003,11.482 15.0003,10.664 14.3003,10 13.5713,10' />
  </DrawingGroup.Children>
</DrawingGroup>");
    public static IconInfo MethodPrivate16 { get; } = new IconInfo("method-private-16", @"<DrawingGroup xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
  <DrawingGroup.Children>
    <GeometryDrawing Brush='#00FFFFFF' Geometry='F1M16,16L0,16 0,0 16,0z' />
    <GeometryDrawing Brush='#FFF6F6F6' Geometry='F1M0.167,8.4258L0.167,2.8568 5.051,0.386799999999999 9.755,2.8678 9.755,8.3808 5.291,11.6538z' />
    <GeometryDrawing Brush='#FFF6F6F6' Geometry='F1M9,16L9.005,12.016C9.005,12.016 8.78,8.031 12.594,8.031 14.126,8.031 16,8.998 16,12.007L16,16z' />
    <GeometryDrawing Brush='#FF642C90' Geometry='F1M8.1904,7.5986L5.6144,9.4546 5.6144,5.9926 8.1904,4.4256z M4.9704,9.5616L4.9704,9.5776 4.9614,9.5696 4.9534,9.5776 4.9534,9.5616 1.7864,7.4966 1.7864,4.1776 4.9954,5.9706 4.9954,9.5436z M4.9954,2.1876L8.0654,3.7016 5.2014,5.4196 2.1064,3.6546z M8.7544,7.8736L8.7544,3.4706 5.0414,1.5116 1.1674,3.4706 1.1674,7.8736 5.2484,10.4446z' />
    <GeometryDrawing Brush='#FFF0EFF1' Geometry='F1M4.9951,2.1875L8.0651,3.7015 5.2011,5.4195 2.1061,3.6545z'>
      <GeometryDrawing.Pen>
        <Pen Brush='#FF642C90' Thickness='0.15000006556510925' DashCap='Flat' />
      </GeometryDrawing.Pen>
    </GeometryDrawing>
    <GeometryDrawing Brush='#FFF0EFF1' Geometry='F1M4.9707,9.5615L4.9707,9.5775 4.9607,9.5695 4.9527,9.5775 4.9527,9.5615 1.7857,7.4975 1.7857,4.1775 4.9947,5.9705 4.9947,9.5435z'>
      <GeometryDrawing.Pen>
        <Pen Brush='#FF642C90' Thickness='0.15000006556510925' DashCap='Flat' />
      </GeometryDrawing.Pen>
    </GeometryDrawing>
    <GeometryDrawing Brush='#FFF0EFF1' Geometry='F1M8.1904,7.5986L5.6144,9.4546 5.6144,5.9926 8.1904,4.4256z'>
      <GeometryDrawing.Pen>
        <Pen Brush='#FF642C90' Thickness='0.15000006556510925' DashCap='Flat' />
      </GeometryDrawing.Pen>
    </GeometryDrawing>
    <GeometryDrawing Brush='#FFF0EFF1' Geometry='F1M12,14L13,14 13,13.041 12,13.041z' />
    <GeometryDrawing Brush='#FF414141' Geometry='F1M11.0186,11.5625C11.0786,10.8335 11.3526,10.0215 12.5936,10.0215 13.8426,10.0215 13.9846,10.8165 13.9976,11.5625L14.9636,11.5625C14.8786,10.4435 14.8206,9.0315 12.5936,9.0315 10.1986,9.0315 10.1486,10.4375 10.0176,11.5625z' />
    <GeometryDrawing Brush='#FF414141' Geometry='F1M13,14L12,14 12,13 13,13z M14,12L11,12 10,12 10,15 15,15 15,12z' />
    <GeometryDrawing Brush='#FFF0EFF1' Geometry='F1M12,14L13,14 13,13 12,13z' />
  </DrawingGroup.Children>
</DrawingGroup>");
    public static IconInfo Namespace16 { get; } = new IconInfo("namespace-16", @"<DrawingGroup xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
  <DrawingGroup.Children>
    <GeometryDrawing Brush='#00FFFFFF' Geometry='F1M16,16L0,16 0,0 16,0z' />
    <GeometryDrawing Brush='#FFF6F6F6' Geometry='F1M5.7388,15.0156C2.6498,14.9806,2.3168,12.8516,2.3168,11.9376L2.3168,9.9206C2.3168,9.5936,2.2498,9.4896,2.2498,9.4886L1.3128,9.4286 1.2498,8.4986 1.2498,6.5626C1.2498,6.5626 2.1898,6.5036 2.1918,6.5036 2.2498,6.5036 2.3168,6.3896 2.3168,6.0516L2.3168,4.0896C2.3168,2.1406,3.5638,1.0086,5.7388,0.984599999999999L6.7498,0.973599999999999 6.7498,4.0196 5.7748,4.0436C5.7298,4.0446 5.6998,4.0486 5.6818,4.0526 5.6878,4.0746 5.6628,4.1676 5.6628,4.3496L5.6628,6.2776C5.6628,6.9526 5.4778,7.5306 5.1298,7.9816 5.4778,8.4316 5.6628,9.0096 5.6628,9.6886L5.6628,11.5956C5.6628,11.7706 5.6768,11.8866 5.6898,11.9586 5.7128,11.9606 6.7498,11.9846 6.7498,11.9846L6.7498,15.0276z M9.2598,11.9846L10.2378,11.9626C10.2738,11.9616 10.3038,11.9596 10.3268,11.9566 10.3378,11.8936 10.3538,11.7786 10.3538,11.5956L10.3538,9.6886C10.3538,9.0096 10.5378,8.4316 10.8838,7.9816 10.5378,7.5306 10.3538,6.9526 10.3538,6.2776L10.3538,4.3496C10.3538,4.1826 10.3328,4.0906 10.3198,4.0476 10.3178,4.0476 9.2598,4.0196 9.2598,4.0196L9.2598,0.973599999999999 10.2708,0.984599999999999C12.4378,1.0086,13.6798,2.1406,13.6798,4.0896L13.6798,6.0516C13.6798,6.4026,13.7508,6.5136,13.7518,6.5146L14.6878,6.5746 14.7398,7.5006 14.7398,9.4366 13.8048,9.4966C13.7288,9.5126,13.6798,9.6436,13.6798,9.9206L13.6798,11.9376C13.6798,12.8516,13.3478,14.9806,10.2718,15.0156L9.2598,15.0276z' />
    <GeometryDrawing Brush='#FF424242' Geometry='F1M13.7397,8.499C13.0337,8.544,12.6797,9.019,12.6797,9.921L12.6797,11.937C12.6797,13.305,11.8737,13.997,10.2607,14.016L10.2607,12.963C10.6567,12.954 10.9377,12.845 11.1037,12.635 11.2707,12.425 11.3537,12.079 11.3537,11.596L11.3537,9.688C11.3537,8.763,11.8027,8.201,12.7007,8L12.7007,7.979C11.8027,7.765,11.3537,7.198,11.3537,6.277L11.3537,4.35C11.3537,3.498,10.9897,3.062,10.2607,3.044L10.2607,1.984C11.8737,2.002,12.6797,2.705,12.6797,4.09L12.6797,6.052C12.6797,6.972,13.0337,7.456,13.7397,7.501z M5.7497,14.016C4.1277,13.997,3.3167,13.305,3.3167,11.937L3.3167,9.921C3.3167,9.019,2.9607,8.544,2.2497,8.499L2.2497,7.501C2.9607,7.456,3.3167,6.972,3.3167,6.052L3.3167,4.09C3.3167,2.705,4.1277,2.002,5.7497,1.984L5.7497,3.044C5.0257,3.062,4.6627,3.498,4.6627,4.35L4.6627,6.277C4.6627,7.198,4.2097,7.765,3.3027,7.979L3.3027,8C4.2097,8.201,4.6627,8.763,4.6627,9.688L4.6627,11.596C4.6627,12.083 4.7437,12.431 4.9057,12.638 5.0677,12.846 5.3487,12.954 5.7497,12.963z' />
  </DrawingGroup.Children>
</DrawingGroup>");
    public static IconInfo Operator16 { get; } = new IconInfo("operator-16", @"<DrawingGroup xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
  <DrawingGroup.Children>
    <GeometryDrawing Brush='#00FFFFFF' Geometry='F1M16,16L0,16 0,0 16,0z' />
    <GeometryDrawing Brush='#FFF6F6F6' Geometry='F1M0,16L16,16 16,0 0,0z' />
    <GeometryDrawing Brush='#FF00529C' Geometry='F1M14,5L9,5 9,4 14,4z M10.281,13L8.719,13 11.719,9 13.281,9z M7,5L5,5 5,7 4,7 4,5 2,5 2,4 4,4 4,2 5,2 5,4 7,4z M7,10L3,10 3,9 7,9z M7,13L3,13 3,12 7,12z M1,15L15,15 15,1 1,1z' />
    <GeometryDrawing Brush='#FFEFEFF0' Geometry='F1M10.2812,13L13.2812,9 11.7182,9 8.7192,13z M7.0002,12L3.0002,12 3.0002,13 7.0002,13z M7.0002,9L3.0002,9 3.0002,10 7.0002,10z M14.0002,4L9.0002,4 9.0002,5 14.0002,5z M7.0002,5L5.0002,5 5.0002,7 4.0002,7 4.0002,5 2.0002,5 2.0002,4 4.0002,4 4.0002,2 5.0002,2 5.0002,4 7.0002,4z' />
  </DrawingGroup.Children>
</DrawingGroup>");
    public static IconInfo Property16 { get; } = new IconInfo("property-16", @"<DrawingGroup xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
  <DrawingGroup.Children>
    <GeometryDrawing Brush='#00FFFFFF' Geometry='F1M16,16L0,16 0,0 16,0z' />
    <GeometryDrawing Brush='#FFF6F6F6' Geometry='F1M16,5.5C16,8.538 13.538,11 10.5,11 10.225,11 9.957,10.973 9.693,10.934 9.667,10.93 9.641,10.927 9.615,10.922 9.337,10.877 9.066,10.816 8.804,10.731L4.268,15.268C3.795,15.74 3.167,16 2.5,16 1.833,16 1.205,15.74 0.731999999999999,15.268 -0.242000000000001,14.293 -0.242000000000001,12.707 0.731999999999999,11.732L5.269,7.196C5.184,6.934 5.123,6.662 5.078,6.384 5.073,6.359 5.07,6.333 5.066,6.307 5.027,6.043 5,5.775 5,5.5 5,2.462 7.462,0 10.5,0 13.538,0 16,2.462 16,5.5' />
    <GeometryDrawing Brush='#FF424242' Geometry='F1M15,5.5C15,7.985 12.985,10 10.5,10 9.807,10 9.158,9.83 8.571,9.55L3.561,14.561C3.268,14.854 2.884,15 2.5,15 2.116,15 1.732,14.854 1.439,14.561 0.853999999999999,13.975 0.853999999999999,13.025 1.439,12.439L6.45,7.429C6.17,6.842 6,6.193 6,5.5 6,3.015 8.015,1 10.5,1 11.193,1 11.842,1.17 12.429,1.45L9.636,4.243 11.757,6.364 14.55,3.571C14.83,4.158,15,4.807,15,5.5' />
  </DrawingGroup.Children>
</DrawingGroup>");
    public static IconInfo XmlTag16 { get; } = new IconInfo("xml-tag-16", @"<DrawingGroup xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
  <DrawingGroup.Children>
    <GeometryDrawing Brush='#00FFFFFF' Geometry='F1M16,16L0,16 0,0 16,0z' />
    <GeometryDrawing Brush='#FFF6F6F6' Geometry='F1M6.7194,5.4529L4.4254,3.1589 0.000399999999999956,7.5849 0.000399999999999956,8.4129 4.4264,12.8399 7.8034,13.9199 8.7244,10.9999 9.7364,10.9999 11.5744,12.8389 16.0004,8.4129 16.0004,7.5849 11.5754,3.1589 11.3904,2.5579 8.2974,1.5799z' />
    <GeometryDrawing Brush='#FF424242' Geometry='F1M5.9639,12.29L7.1509,12.665 10.1359,3.21 8.9499,2.835z M11.5749,4.573L10.6939,5.453 13.2409,7.999 10.6939,10.544 11.5749,11.425 14.9999,7.999z M5.3049,5.453L2.7589,7.999 5.3049,10.544 4.4249,11.425 0.999899999999999,7.999 4.4249,4.573z' />
  </DrawingGroup.Children>
</DrawingGroup>");

    static Icons()
    {
      All = new IconInfo[] {
        AbstractClass16,
        Assembly16,
        Attribute16,
        Class16,
        ClassCollection16,
        ClassMoved16,
        DatabaseMethod16,
        EnumValue16,
        Event16,
        Field16,
        Folder16,
        FolderSpecial16,
        Method16,
        MethodFriend16,
        MethodPrivate16,
        Namespace16,
        Operator16,
        Property16,
        XmlTag16
      };
    }

    public static BitmapImage ToWpf(this System.Drawing.Image img)
    {
      var ms = new MemoryStream();
      img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
      var result = new BitmapImage();
      result.BeginInit();
      ms.Seek(0, SeekOrigin.Begin);
      result.StreamSource = ms;
      result.EndInit();
      return result;
    }

    public static IconInfo ForItemType(ItemType itemType)
    {
      if (itemType.IsPolymorphic)
        return AbstractClass16;
      if (itemType.IsFederated)
        return ClassMoved16;
      if (itemType.IsRelationship)
        return ClassCollection16;
      return Class16;
    }

    public static IconInfo ForMethod(Method method)
    {
      if (method.IsServerEvent)
        return Event16;
      if ((method.ExecutionAllowedTo ?? "World") != "World")
        return MethodPrivate16;
      return Method16;
    }

    public static DrawingImage DrawingStringToImage(string xaml)
    {
      using (var reader = new StringReader(xaml))
      using (var xml = XmlReader.Create(reader))
      {
        var drawing = (Drawing)XamlReader.Load(xml);
        return new DrawingImage(drawing);
      }
    }

    public static System.Drawing.Bitmap ToGdi(this DrawingImage image)
    {
      var visual = new DrawingVisual();
      using (var dc = visual.RenderOpen())
      {
        dc.DrawDrawing(image.Drawing);
        dc.Close();
      }
      var target = new RenderTargetBitmap((int)visual.Drawing.Bounds.Right, (int)visual.Drawing.Bounds.Bottom, 96.0, 96.0, PixelFormats.Pbgra32);
      target.Render(visual);

      using (var stream = new MemoryStream())
      {
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(target));
        encoder.Save(stream);
        stream.Position = 0;
        return new System.Drawing.Bitmap(stream);
      }
    }
  }


  public struct IconInfo
  {
    public System.Drawing.Image Gdi { get; set; }
    public string Key { get; set; }
    public ImageSource Wpf { get; set; }

    public IconInfo(string key, string xamlDrawing)
    {
      var drawingImage = Icons.DrawingStringToImage(xamlDrawing);
      Wpf = drawingImage;
      Key = key;
      Gdi = drawingImage.ToGdi();
    }

    public IconInfo(string key, System.Drawing.Image img)
    {
      Gdi = img;
      Key = key;
      Wpf = img.ToWpf();
    }
  }
}
