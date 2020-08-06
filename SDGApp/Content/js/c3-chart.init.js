$(function () {
var chart = c3.generate({

    bindto: '#chart',

    data: {
    columns: [
            ['data1', -0.062084443867206573, -0.1457425355911255, -0.1462487131357193, -0.02475827746093273, 0.11038841307163239, 0.12685908377170563, 0.029849981889128685, -0.058143552392721176, -0.036348000168800354, 0.05762411653995514, 0.10842691361904144, 0.07339297235012054, 0.0039032772183418274, -0.01705998368561268, 0.023052167147397995, 0.08868519216775894, 0.15705591440200806, 0.1904146671295166, 0.10606028139591217, -0.18643534183502197, -0.6008145213127136, -0.7701438069343567, -0.2604771554470062, 1.017932415008545, 2.5332698822021484, 3.3818390369415283, 2.9190478324890137, 1.2512924671173096, -0.8075845837593079, -2.3154096603393555, -2.778465509414673, -2.322251081466675, -1.4437962770462036, -0.6163633465766907, -0.09783145785331726, 0.08610910922288895, 0.050007402896881104, -0.04822898283600807, -0.07844873517751694, -0.02222856692969799, 0.04354432597756386, 0.04525778070092201, -0.007815727964043617, -0.022469058632850647, 0.037204157561063766, 0.10010712593793869, 0.0629589781165123, -0.07321906089782715, -0.16678057610988617, -0.10165325552225113, 0.07901693135499954, 0.2012593299150467, 0.1482643038034439, -0.0033967706840485334, -0.08773834258317947, -0.026519346982240677, 0.09845928102731705, 0.15776920318603516, 0.12613411247730255, 0.059571217745542526, 0.0013670391635969281, -0.06703571230173111, -0.1567053496837616, -0.19837258756160736, -0.13007546961307526, 0.011828827671706676, 0.09565935283899307, 0.04805244132876396, -0.04283776134252548, -0.04681140184402466, 0.0485343262553215, 0.10296769440174103, 0.006957916542887688, -0.1497269719839096, -0.17756851017475128, 9.472600650042295E-4, 0.2302643358707428, 0.2892906963825226, 0.13659098744392395, -0.06873372942209244, -0.14194777607917786, -0.07263628393411636, 0.008130417205393314, 0.007489587180316448, -0.03958849608898163, -0.035241495817899704, 0.033176835626363754, 0.0847802609205246, 0.05522305890917778, -0.03234716132283211, -0.0901251807808876, -0.08497366309165955, -0.0629570484161377, -0.07561292499303818, -0.11894489824771881, -0.12684234976768494, -0.06957468390464783, 0.021430151537060738, 0.10044532269239426, 0.15694431960582733, 0.21903696656227112, 0.26470544934272766, 0.2216428816318512, 0.05377728119492531, -0.14504234492778778, -0.1797446608543396, 0.035945553332567215, 0.34052178263664246, 0.40621864795684814, 0.03736044466495514, -0.5614253878593445, -0.8653949499130249, -0.40105143189430237, 0.8370152711868286, 2.2724199295043945, 3.060291051864624, 2.606276035308838, 0.989742636680603, -1.044194221496582, -2.518007278442383, -2.8079230785369873, -2.0183863639831543, -0.8300795555114746, 0.008199991658329964, 0.17423026263713837, -0.09814179688692093, -0.3259560465812683, -0.17006351053714752, 0.34357979893684387, 0.9051774144172668, 1.1672723293304443, 0.9332754611968994, 0.27011144161224365, -0.5345191359519958, -1.0992997884750366, -1.1532008647918701, -0.7213581800460815, -0.11328917741775513, 0.28353530168533325, 0.2941543161869049, 0.06777302175760269, -0.09787347912788391, -0.02296322211623192, 0.20946328341960907, 0.3844028413295746, 0.3897983431816101, 0.3064286410808563, 0.30406755208969116, 0.43536388874053955, 0.5810741186141968, 0.5668448805809021, 0.3120087683200836, -0.09765328466892242, -0.505279004573822, -0.7835274934768677, -0.8742398023605347, -0.7814466953277588, -0.5374684929847717, -0.22849664092063904, 0.027515413239598274, 0.1413731575012207, 0.11503994464874268, 0.05538569390773773, 0.06102373078465462, 0.1406029909849167, 0.20585840940475464, 0.16156859695911407, 0.013641776517033577, -0.13936762511730194, -0.1740512251853943, -0.03825250640511513, 0.215751513838768, 0.4693335294723511, 0.5841343402862549, 0.48261117935180664, 0.18650248646736145, -0.18370521068572998, -0.463468462228775, -0.5550382137298584, -0.47163328528404236, -0.3129725754261017, -0.1668345183134079, -0.022974219173192978, 0.20037740468978882, 0.5861915946006775, 1.1192288398742676, 1.6666494607925415, 2.048196315765381, 2.1189141273498535, 1.8568288087844849, 1.3591481447219849, 0.7889751195907593, 0.2927607297897339, -0.08124348521232605, -0.35840412974357605, -0.5839080214500427, -0.7354382276535034, -0.7211965322494507, -0.4965576231479645, -0.16988074779510498, -0.014204371720552444, -0],
            ['data2', 0.0, 6.872571428571429, 13.745142857142858, 20.617714285714285, 27.490285714285715, 34.362857142857145, 41.23542857142857, 48.108, 54.98057142857142, 61.85314285714285, 68.72571428571428, 75.59828571428571, 82.47085714285714, 89.34342857142857, 96.21600000000001, 103.08857142857144, 109.96114285714287, 116.83371428571431, 123.70628571428574, 130.57885714285717, 137.4514285714286, 144.32400000000004, 151.19657142857147, 158.0691428571429, 164.94171428571434, 171.81428571428577, 178.6868571428572, 185.55942857142864, 192.43200000000007, 199.3045714285715, 206.17714285714294, 213.04971428571437, 219.9222857142858, 226.79485714285724, 233.66742857142867, 240.5400000000001, 247.41257142857154, 254.28514285714297, 261.1577142857144, 268.0302857142858, 274.9028571428572, 281.7754285714286, 288.648, 295.52057142857143, 302.39314285714283, 309.26571428571424, 316.13828571428564, 323.01085714285705, 329.88342857142845, 336.75599999999986, 343.62857142857126, 350.50114285714267, 357.3737142857141, 364.2462857142855, 371.1188571428569, 377.9914285714283, 384.8639999999997, 391.7365714285711, 398.6091428571425, 405.4817142857139, 412.3542857142853, 419.2268571428567, 426.0994285714281, 432.9719999999995, 439.84457142857093, 446.71714285714233, 453.58971428571374, 460.46228571428514, 467.33485714285655, 474.20742857142795, 481.07999999999936, 487.95257142857076, 494.82514285714217, 501.6977142857136, 508.570285714285, 515.4428571428564, 522.3154285714278, 529.1879999999992, 536.0605714285706, 542.933142857142, 549.8057142857134, 556.6782857142848, 563.5508571428562, 570.4234285714276, 577.295999999999, 584.1685714285704, 591.0411428571418, 597.9137142857132, 604.7862857142846, 611.658857142856, 618.5314285714275, 625.4039999999989, 632.2765714285703, 639.1491428571417, 646.0217142857131, 652.8942857142845, 659.7668571428559, 666.6394285714273, 673.5119999999987, 680.3845714285701, 687.2571428571415, 694.1297142857129, 701.0022857142843, 707.8748571428557, 714.7474285714271, 721.6199999999985, 728.4925714285699, 735.3651428571413, 742.2377142857127, 749.1102857142841, 755.9828571428556, 762.855428571427, 769.7279999999984, 776.6005714285698, 783.4731428571412, 790.3457142857126, 797.218285714284, 804.0908571428554, 810.9634285714268, 817.8359999999982, 824.7085714285696, 831.581142857141, 838.4537142857124, 845.3262857142838, 852.1988571428552, 859.0714285714266, 865.943999999998, 872.8165714285694, 879.6891428571408, 886.5617142857122, 893.4342857142836, 900.306857142855, 907.1794285714265, 914.0519999999979, 920.9245714285693, 927.7971428571407, 934.6697142857121, 941.5422857142835, 948.4148571428549, 955.2874285714263, 962.1599999999977, 969.0325714285691, 975.9051428571405, 982.7777142857119, 989.6502857142833, 996.5228571428547, 1003.3954285714261, 1010.2679999999975, 1017.1405714285689, 1024.0131428571403, 1030.8857142857119, 1037.7582857142834, 1044.630857142855, 1051.5034285714264, 1058.375999999998, 1065.2485714285694, 1072.121142857141, 1078.9937142857125, 1085.866285714284, 1092.7388571428555, 1099.611428571427, 1106.4839999999986, 1113.35657142857, 1120.2291428571416, 1127.1017142857131, 1133.9742857142846, 1140.8468571428562, 1147.7194285714277, 1154.5919999999992, 1161.4645714285707, 1168.3371428571422, 1175.2097142857137, 1182.0822857142853, 1188.9548571428568, 1195.8274285714283, 1202.6999999999998, 1209.5725714285713, 1216.4451428571429, 1223.3177142857144, 1230.190285714286, 1237.0628571428574, 1243.935428571429, 1250.8080000000004, 1257.680571428572, 1264.5531428571435, 1271.425714285715, 1278.2982857142865, 1285.170857142858, 1292.0434285714296, 1298.916000000001, 1305.7885714285726, 1312.6611428571441, 1319.5337142857156, 1326.4062857142872, 1333.2788571428587, 1340.1514285714302, 1347.0240000000017, 1353.8965714285732, 1360.7691428571447, 1367.6417142857163, 1374.5142857142878, 1381.3868571428593, 1388.2594285714308, 1395.1320000000023, 1402.0045714285739, 1408.8771428571454, 1415.749714285717, 1422.6222857142884, 1429.49485714286, 1436.3674285714314, 1443.240000000003, 1450.1125714285745, 1456.985142857146, 1463.8577142857175, 1470.730285714289, 1477.6028571428606, 1484.475428571432, 1491.3480000000036, 1498.220571428575, 150.0931428571466, 1511.9657142857181, 1518.8382857142897, 1525.0]
    ],
    types: {
    data1: 'line',
    data2: 'line'
    }
},

axis: {
    x: {
    type: 'categorized'
    }
}

});



});
$(function () {
var chart = c3.generate({
    bindto: '#combine-chart',
    data: {
        columns: [
            ['data1', 30, 20, 50, 40, 60, 50],
            ['data2', 200, 130, 90, 240, 130, 220],
            ['data3', 300, 200, 160, 400, 250, 250],
            ['data4', 200, 130, 90, 240, 130, 220],
            ['data5', 130, 120, 150, 140, 160, 150]
        ],
        types: {
            data1: 'bar',
            data2: 'bar',
            data3: 'spline',
            data4: 'line',
            data5: 'bar'
        },
        groups: [
            ['data1','data2']
        ]
    },
    axis: {
        x: {
            type: 'categorized'
        }
    }
});

});
    $(function () {
    var chart = c3.generate({
        bindto: '#roated-chart',
        data: {
        columns: [
        ['data1', 30, 200, 100, 400, 150, 250],
        ['data2', 50, 20, 10, 40, 15, 25]
        ],
        types: {
        data1: 'bar'
        }
    },
    axis: {
        rotated: true,
        x: {
        type: 'categorized'
        }
    }
    });
    });