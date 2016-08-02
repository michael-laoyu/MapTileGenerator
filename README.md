# MapTileGenerator
结合WMS服务的切图工具

##使用说明：
设置mapConfig.json，根据配置项请求WMS服务进行切片，支持多线程方式。


	{
	
		"resolutions" :   [128, 64,32], 
		
		"tileSize" : [256,256],
		
		"extent" : [12662882.864568064,2543518.577818389,12761187.939702341,2599303.8005401082], 
		
		"origin": [0,0],
		
		"wmsUrl": "http://localhost:8080/geoserver/szgas/wms",
		
		"offsetZoom" :11, //ZOOM级别的偏移值，保存瓦片时以zoom + offsetZoom保存路径;
		
		"wmsParas" : {
		
			"FORMAT" : "image/png",
			
			"VERSION" : "1.1.1",
			
			"STYLES" : "",
			
			"LAYERS" : "szgas:jd_baidu",
			
			"REQUEST" : "GetMap",
			
			"SRS" : "EPSG:3857",
			
			"TRANSPARENT" : true
			
		},
		"runThreadCount" : 5,
		
		"savePath" : ""  //如果不设置保存路径，则保存在程序根目录\Tiles下;

	}

##瓦片规则
瓦片存储路径：程序目录\Tiles\Zoom+offsetZoom\x_y.png
![Paste_Image.png](http://upload-images.jianshu.io/upload_images/2137628-204d853cce816a7d.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

![Paste_Image.png](http://upload-images.jianshu.io/upload_images/2137628-77c29b8d13114922.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
