/* KKKH060Pjs  名刺データインポート*/
var KKKH060Pjs = KKKH060Pjs || (function () {
    /* const定義 */
    var c_myLayoutId = "KKKH060P";
    var c_ctrlPrefix = "#" + c_myLayoutId + "_";

		
	/* 条件欄の初期化 */
	function setInitCondCtrl() {
		var setData = { 
			dtSc00001		: "",								 // 取得日
			clSc00003		: "",								 // 担当者
			clSc00002		: ""								// 顧客
		};
		$.rwat.api.setDataToLayout(c_myLayoutId, setData);
	}
	
	var HeaderPagingParam = {
			lbCurS		: "lbCurrentCntS",	    // 先頭行番号ラベル
			lbCurE		: "lbCurrentCntE",	    // 末尾行番号ラベル
			lbTotal		: "lbTotalCnt",		    // 総件数ラベル
			hlbPerPage	: "hlbCntPerPage",	    // (隠し)ページ毎の表示件数
			hlbLineS	: "hlbGetLineS",	    // (隠し)リクエスト用先頭行
			hlbLineE	: "hlbGetLineE",	    // (隠し)リクエスト用末尾行
			btFirst		: "btHdrLsFirst",		// <<ボタン
			btPrev		: "btHdrLsBack",		// <ボタン
			btNext		: "btHdrLsNext",		// >ボタン
			btLast		: "btHdrLsLast"			// >>ボタン
	};
	
	var FooterPagingParam = {
			lbCurS		: "lbCurrentCntS",	    // 先頭行番号ラベル
			lbCurE		: "lbCurrentCntE",	    // 末尾行番号ラベル
			lbTotal		: "lbTotalCnt",		    // 総件数ラベル
			hlbPerPage	: "hlbCntPerPage",	    // (隠し)ページ毎の表示件数
			hlbLineS	: "hlbGetLineS",	    // (隠し)リクエスト用先頭行
			hlbLineE	: "hlbGetLineE",	    // (隠し)リクエスト用末尾行
			btFirst		: "btLsFirst",			// <<ボタン
			btPrev		: "btLsBack",			// <ボタン
			btNext		: "btLsNext",			// >ボタン
			btLast		: "btLsLast"			// >>ボタン
	};
	
	// 検索結果選択位置
	var p_index = -1;
    /* publicメソッド */
    return {
        /* E01:初期起動処理 */
        P01_onload_start: function (evt) {
            //処理なし
			// ページングの初期化
			$.erp_cmn.setBeforePaging(c_myLayoutId, "init", HeaderPagingParam);
			$.erp_cmn.setBeforePaging(c_myLayoutId, "init", FooterPagingParam);
        },

        P01_onload_finish: function (evt) {
            //処理なし
			// ページング結果の反映
			$.erp_cmn.setAfterPaging(c_myLayoutId, HeaderPagingParam);
			$.erp_cmn.setAfterPaging(c_myLayoutId, FooterPagingParam);
			selectText();
			$(c_ctrlPrefix+"btSetMeishiImport").rwatDisabled(true);  
			$(c_ctrlPrefix+"btSetKokyaku").rwatDisabled(true);   
        },
		//マスタ担当者更新処理
        /*E02:ファイルアップロード処理*/
        btMeishiImport_onclick_start: function (evt) {
        	errorClear();
            //ファイル選択チェック
            var fsFileValue = $(c_ctrlPrefix + "fsFile").val();
            if (fsFileValue === "") {
                evt.stopProcess = true;
				// メッセージ「CO4048W：#{0}を選択してください。（#{0}：”ファイル”）」を出力し、
                // リターンする
                $.rwat.msgbox.showMsgById("CO4048W", ["ファイル"]);
                return;
            };
			// メッセージ「C00006I：#{0}を実行します。よろしいですか？(#{0}：”インポート”」を
            $.rwat.msgbox.showMsgById("CO0006I", ["インポート"]).onOK(function () {            	
                // 実行イベント
                var reader = new FileReader();
				var blob = $(c_ctrlPrefix + "fsFile")[0].files[0];				
				reader.onload = function(){
					$("#" + evt.targetId).trigger("execute");
				};
				reader.onerror = function(){
					$.rwat.msgbox.showMsgById("KK2002W");
               		return;
				};
				reader.readAsText(blob); 
				$(c_ctrlPrefix+"btSetMeishiImport").rwatDisabled(false); 
       			$(c_ctrlPrefix+"btSetKokyaku").rwatDisabled(false);                  
            });
			// ページングの初期化
			$.erp_cmn.setBeforePaging(c_myLayoutId, "init", HeaderPagingParam);
			$.erp_cmn.setBeforePaging(c_myLayoutId, "init", FooterPagingParam);			
        },

        btMeishiImport_onclick_finish: function (evt) {
       		
        },

        /*アップロード関数*/
        btMeishiImport_onexecute_start: function (evt) {
            //画面．ファイルアップロードのアップロード関数(rwatUpload)を呼び出す
            $(c_ctrlPrefix + "upFile").rwatUpload();           
        },

        /*アップロード関数*/
        btMeishiImport_onexecute_finish: function (evt) {
           
            selectText();
        },
        
        /*メッセージを出力する*/
        upFile_onfail_start: function (evt) {
            // メッセージ「CO2027E：ファイルのアップロードに失敗しました。
            // システム管理者に報告してください。」を出力する
            $.rwat.msgbox.showMsgById("CO2027E");
        },

        /*アップロード成功ファイルURL取得*/
        upFile_onsuccess_start: function (evt) {
            var upImportFilePath = $(c_ctrlPrefix + "upFile").getUploadUrls();
            $(c_ctrlPrefix + "hlbUploadFileURL").rwatVal(upImportFilePath);
            selectText();
        },

        upFile_onsuccess_finish: function (evt) {
			
			 // ページング後処理を行う
			$.erp_cmn.setAfterPaging(c_myLayoutId, HeaderPagingParam);
			$.erp_cmn.setAfterPaging(c_myLayoutId, FooterPagingParam);

			selectText();
            // サーバから送信された選択状態を結果一覧に反映する
            setListSelected();
            //処理なし
		
        },
		
		/* 結果一覧ページング */
		btLsFirst_onclick_start: function(evt) {
			// 最初のページ
			$.erp_cmn.setBeforePaging(c_myLayoutId, "first", FooterPagingParam);
		},

		btLsFirst_onclick_finish: function(evt) {
			// ページング結果の反映
			$.erp_cmn.setAfterPaging(c_myLayoutId, HeaderPagingParam);
			$.erp_cmn.setAfterPaging(c_myLayoutId, FooterPagingParam);
			p_index = -1;
			selectText();
		},

		btLsBack_onclick_start: function(evt) {
			// 現在の前ページ
			$.erp_cmn.setBeforePaging(c_myLayoutId, "prev", FooterPagingParam);
		},

		btLsBack_onclick_finish: function(evt) {
			// ページング結果の反映
			$.erp_cmn.setAfterPaging(c_myLayoutId, HeaderPagingParam);
			$.erp_cmn.setAfterPaging(c_myLayoutId, FooterPagingParam);
			p_index = -1;
			selectText();
		},

		btLsNext_onclick_start: function(evt) {
			// 現在の次ページ
			$.erp_cmn.setBeforePaging(c_myLayoutId, "next", FooterPagingParam);
		},

		btLsNext_onclick_finish: function(evt) {
			// ページング結果の反映
			$.erp_cmn.setAfterPaging(c_myLayoutId, HeaderPagingParam);
			$.erp_cmn.setAfterPaging(c_myLayoutId, FooterPagingParam);
			p_index = -1;
			selectText();
		},

		btLsLast_onclick_start: function(evt) {
			// 最終のページ
			$.erp_cmn.setBeforePaging(c_myLayoutId, "last",FooterPagingParam);
		},

		btLsLast_onclick_finish: function(evt) {
			// ページング結果の反映
			$.erp_cmn.setAfterPaging(c_myLayoutId, HeaderPagingParam);
			$.erp_cmn.setAfterPaging(c_myLayoutId, FooterPagingParam);
			p_index = -1;
			selectText();
		},

		/* 検索条件クリア処理 */
		btClear_onclick_start: function(evt) {
			// 検索条件を初期化
			setInitCondCtrl();
		},		

		btClear_onclick_finish: function(evt) {
			// 検索条件を初期化
			setInitCondCtrl();
		},

		btSetKokyaku_onclick_start: function(evt) {
		},

		btSetKokyaku_onclick_finish: function(evt) {
			// ページング結果の反映
			$.erp_cmn.setAfterPaging(c_myLayoutId, HeaderPagingParam);
			$.erp_cmn.setAfterPaging(c_myLayoutId, FooterPagingParam);
			p_index = -1;
			selectText();
		},

		btSetMeishiImport_onclick_start: function(evt) {    
		 	//ファイル存在チェック
            var CntsValue = $(c_ctrlPrefix + "lbTotalCnt").val();  
			if (CntsValue + 0 > 0) {
                evt.stopProcess = true;
				// メッセージ「CO4048W：#{0}を選択してください。（#{0}：”ファイル”）」を出力し、
                // リターンする
                $.rwat.msgbox.showMsgById("CO4048W", ["ファイル"]);
                return;
            };    
		},
		
		btSetMeishiImport_onclick_finish: function(evt) {
		  //実行イベント
          $(c_ctrlPrefix + "btUpdateMeishiData").trigger("execute");
		},

		btUpdateMeishiData_onexecute_start: function(evt) {
         
		},

		btUpdateMeishiData_onexecute_finish: function(evt) {	
		  //実行イベント
          $(c_ctrlPrefix + "btCheckMeishiImport").trigger("execute");	
		},
		
		/*名刺登録チェック*/
        btCheckMeishiImport_onexecute_start: function (evt) {          
        
        },

        /*名刺登録チェック*/
        btCheckMeishiImport_onexecute_finish: function (evt) {           
           	// ページング結果の反映
			$.erp_cmn.setAfterPaging(c_myLayoutId, HeaderPagingParam);
			$.erp_cmn.setAfterPaging(c_myLayoutId, FooterPagingParam);
			
			 if($(c_ctrlPrefix+"hlErrorLine1").rwatVal() + 0 > 0) {
				 $.rwat.msgbox.showMsgById("KK2006W",$(c_ctrlPrefix+"hlErrorLine1").rwatVal());
				 	selectText();
               		return;
			 }
			  if($(c_ctrlPrefix+"hlErrorLine2").rwatVal() + 0 > 0) {
			 	$.rwat.msgbox.showMsgById("KK2007W",$(c_ctrlPrefix+"hlErrorLine2").rwatVal());
               		return;
			 }
			  if($(c_ctrlPrefix+"hlErrorLine4").rwatVal() + 0 > 0) {
			 	$.rwat.msgbox.showMsgById("KK2009W",$(c_ctrlPrefix+"hlErrorLine4").rwatVal());
               		return;
			 }
			//登録を続行しますか
			 if($(c_ctrlPrefix+"hlErrorLine3").rwatVal() + 0 > 0) {
			  $.rwat.msgbox.showMsgById("KK2008W",$(c_ctrlPrefix+"hlErrorLine3").rwatVal(),"CANCEL").onOK(function () {
         		 //実行イベント
           	 	 	 $(c_ctrlPrefix+"btSetMeishiImport").trigger("execute");
           		 });
			 }
			//登録を実行します。よろしいですか？
		    else{
		   		 $.rwat.msgbox.showMsgById("KK0001I").onOK(function () {
           		     // 実行イベント
           		    $(c_ctrlPrefix + "btSetMeishiImport").trigger("execute");
           		 }); 
                };         
			p_index = -1;
			selectText();
        },	
			
		/*名刺登録*/
        btSetMeishiImport_onexecute_start: function (evt) {          
        },

        /*名刺登録*/
        btSetMeishiImport_onexecute_finish: function (evt) {           
           	// ページング結果の反映
			$.erp_cmn.setAfterPaging(c_myLayoutId, HeaderPagingParam);
			$.erp_cmn.setAfterPaging(c_myLayoutId, FooterPagingParam);
			p_index = -1;
			selectText();
			errorClear();
			dataClear();
			$.rwat.msgbox.showMsgById("KK1002I");			
        },	
        
		btHdrLsFirst_onclick_start: function(evt) {
			// 最初のページ
			$.erp_cmn.setBeforePaging(c_myLayoutId, "first", HeaderPagingParam);
		},

		btHdrLsFirst_onclick_finish: function(evt) {
			// ページング結果の反映
			$.erp_cmn.setAfterPaging(c_myLayoutId, HeaderPagingParam);
			$.erp_cmn.setAfterPaging(c_myLayoutId, FooterPagingParam);
			p_index = -1;
			selectText();
		},

		btHdrLsBack_onclick_start: function(evt) {
			// 現在の前ページ
			$.erp_cmn.setBeforePaging(c_myLayoutId, "prev", HeaderPagingParam);
		},

		btHdrLsBack_onclick_finish: function(evt) {
			// ページング結果の反映
			$.erp_cmn.setAfterPaging(c_myLayoutId, HeaderPagingParam);
			$.erp_cmn.setAfterPaging(c_myLayoutId, FooterPagingParam);
			p_index = -1;
			selectText();
		},

		btHdrLsNext_onclick_start: function(evt) {
			// 現在の次ページ
			$.erp_cmn.setBeforePaging(c_myLayoutId, "next", HeaderPagingParam);
		},

		btHdrLsNext_onclick_finish: function(evt) {
			// ページング結果の反映
			$.erp_cmn.setAfterPaging(c_myLayoutId, HeaderPagingParam);
			$.erp_cmn.setAfterPaging(c_myLayoutId, FooterPagingParam);
			p_index = -1;
			selectText();
		},

		btHdrLsLast_onclick_start: function(evt) {
			// 最終のページ
			$.erp_cmn.setBeforePaging(c_myLayoutId, "last", HeaderPagingParam);
		},

		btHdrLsLast_onclick_finish: function(evt) {
			// ページング結果の反映
			$.erp_cmn.setAfterPaging(c_myLayoutId, HeaderPagingParam);
			$.erp_cmn.setAfterPaging(c_myLayoutId, FooterPagingParam);
			p_index = -1;
			selectText();
		},
		
		/*相手先担当者1コードリブ入力*/
		lsiAitesakiTantoshaCd1_onset_start: function(evt) {	
			$(c_ctrlPrefix+"shinkiMsg1").addClass("disnon");
		},
		
        lsiAitesakiTantoshaCd1_onset_finish: function (evt) {          
        	//処理なし
        },
        
        /*相手先担当者1コードリブクリア*/
		lsiAitesakiTantoshaCd1_onclear_start: function(evt) {	
			$(c_ctrlPrefix+"shinkiMsg1").removeClass("disnon");
		},
		
        lsiAitesakiTantoshaCd1_onclear_finish: function (evt) {          
        	//処理なし
        },
        
        /*相手先担当者2コードリブ入力*/
		lsiAitesakiTantoshaCd2_onset_start: function(evt) {	
			$(c_ctrlPrefix+"shinkiMsg2").addClass("disnon");
		},
		
        lsiAitesakiTantoshaCd2_onset_finish: function (evt) {          
        	//処理なし
        },
        
        /*相手先担当者2コードリブクリア*/
		lsiAitesakiTantoshaCd2_onclear_start: function(evt) {	
			$(c_ctrlPrefix+"shinkiMsg2").removeClass("disnon");
		},
		
        lsiAitesakiTantoshaCd2_onclear_finish: function (evt) {          
        	//処理なし
        },
        
        /*相手先担当者3コードリブ入力*/
		lsiAitesakiTantoshaCd3_onset_start: function(evt) {	
			$(c_ctrlPrefix+"shinkiMsg3").addClass("disnon");
		},
		
        lsiAitesakiTantoshaCd3_onset_finish: function (evt) {          
        	//処理なし
        },
        
        /*相手先担当者3コードリブクリア*/
		lsiAitesakiTantoshaCd3_onclear_start: function(evt) {	
			$(c_ctrlPrefix+"shinkiMsg3").removeClass("disnon");
		},
		
        lsiAitesakiTantoshaCd3_onclear_finish: function (evt) {          
        	//処理なし
        },
        
        /*相手先担当者4コードリブ入力*/
		lsiAitesakiTantoshaCd4_onset_start: function(evt) {	
			$(c_ctrlPrefix+"shinkiMsg4").addClass("disnon");
		},
		
        lsiAitesakiTantoshaCd4_onset_finish: function (evt) {          
        	//処理なし
        },
        
        /*相手先担当者4コードリブクリア*/
		lsiAitesakiTantoshaCd4_onclear_start: function(evt) {	
			$(c_ctrlPrefix+"shinkiMsg4").removeClass("disnon");
		},
		
        lsiAitesakiTantoshaCd4_onclear_finish: function (evt) {          
        	//処理なし
        },
        
        /*相手先担当者5コードリブ入力*/
		lsiAitesakiTantoshaCd5_onset_start: function(evt) {	
			$(c_ctrlPrefix+"shinkiMsg5").addClass("disnon");
		},
		
        lsiAitesakiTantoshaCd5_onset_finish: function (evt) {          
        	//処理なし
        },
        
        /*相手先担当者5コードリブクリア*/
		lsiAitesakiTantoshaCd5_onclear_start: function(evt) {	
			$(c_ctrlPrefix+"shinkiMsg5").removeClass("disnon");
		},
		
        lsiAitesakiTantoshaCd5_onclear_finish: function (evt) {          
        	//処理なし
        },
        
        /*相手先担当者6コードリブ入力*/
		lsiAitesakiTantoshaCd6_onset_start: function(evt) {	
			$(c_ctrlPrefix+"shinkiMsg6").addClass("disnon");
		},
		
        lsiAitesakiTantoshaCd6_onset_finish: function (evt) {          
        	//処理なし
        },
        
        /*相手先担当者6コードリブクリア*/
		lsiAitesakiTantoshaCd6_onclear_start: function(evt) {	
			$(c_ctrlPrefix+"shinkiMsg6").removeClass("disnon");
		},
		
        lsiAitesakiTantoshaCd6_onclear_finish: function (evt) {          
        	//処理なし
        },
        
        /*相手先担当者7コードリブ入力*/
		lsiAitesakiTantoshaCd7_onset_start: function(evt) {	
			$(c_ctrlPrefix+"shinkiMsg7").addClass("disnon");
		},
		
        lsiAitesakiTantoshaCd7_onset_finish: function (evt) {          
        	//処理なし
        },
        
        /*相手先担当者7コードリブクリア*/
		lsiAitesakiTantoshaCd7_onclear_start: function(evt) {	
			$(c_ctrlPrefix+"shinkiMsg7").removeClass("disnon");
		},
		
        lsiAitesakiTantoshaCd7_onclear_finish: function (evt) {          
        	//処理なし
        },
        
        /*相手先担当者8コードリブ入力*/
		lsiAitesakiTantoshaCd8_onset_start: function(evt) {	
			$(c_ctrlPrefix+"shinkiMsg8").addClass("disnon");
		},
		
        lsiAitesakiTantoshaCd8_onset_finish: function (evt) {          
        	//処理なし
        },
        
        /*相手先担当者8コードリブクリア*/
		lsiAitesakiTantoshaCd8_onclear_start: function(evt) {	
			$(c_ctrlPrefix+"shinkiMsg8").removeClass("disnon");
		},
		
        lsiAitesakiTantoshaCd8_onclear_finish: function (evt) {          
        	//処理なし
        },
        
        /*相手先担当者9コードリブ入力*/
		lsiAitesakiTantoshaCd9_onset_start: function(evt) {	
			$(c_ctrlPrefix+"shinkiMsg9").addClass("disnon");
		},
		
        lsiAitesakiTantoshaCd9_onset_finish: function (evt) {          
        	//処理なし
        },
        
        /*相手先担当者9コードリブクリア*/
		lsiAitesakiTantoshaCd9_onclear_start: function(evt) {	
			$(c_ctrlPrefix+"shinkiMsg9").removeClass("disnon");
		},
		
        lsiAitesakiTantoshaCd9_onclear_finish: function (evt) {          
        	//処理なし
        },
        
        /*相手先担当者10コードリブ入力*/
		lsiAitesakiTantoshaCd10_onset_start: function(evt) {	
			$(c_ctrlPrefix+"shinkiMsg10").addClass("disnon");
		},
		
        lsiAitesakiTantoshaCd10_onset_finish: function (evt) {          
        	//処理なし
        },
        
        /*相手先担当者10コードリブクリア*/
		lsiAitesakiTantoshaCd10_onclear_start: function(evt) {	
			$(c_ctrlPrefix+"shinkiMsg10").removeClass("disnon");
		},
		
        lsiAitesakiTantoshaCd10_onclear_finish: function (evt) {          
        	//処理なし
        },

    };
    
    /* 結果一覧初期調整 */
	function selectText() {
		$(c_ctrlPrefix+"lsiLineNo1").parents("table").removeClass("disnon");
		$(c_ctrlPrefix+"lsiLineNo1").parents("table").find("tr").removeClass("disnon");
		if($(c_ctrlPrefix+"hlErrorflg").rwatVal() + 0 > 0 || $(c_ctrlPrefix+"hlErrorLine1").rwatVal() + 0 > 0) {
			$(c_ctrlPrefix+"CSVErrorMsg").removeClass("disnon");
		}
		else {
			$(c_ctrlPrefix+"CSVErrorMsg").addClass("disnon");
		}
		for (var i = 1; i <= 10; i++) {
			if ($(c_ctrlPrefix+"lsiLineNo"+i).text() ? false : true ){
    			$(c_ctrlPrefix+"lsiLineNo"+i).parent().parent().nextAll().addClass("disnon");	
    			$(c_ctrlPrefix+"lsiLineNo"+i).parent().parent().addClass("disnon");
    			return;  		 		
    		}
    		if($(c_ctrlPrefix+"lsiAitesakiTantoshaCd"+i).rwatVal() == "" && $(c_ctrlPrefix+"lsiAitesakiTantoshaNM"+i).rwatVal() != "") {
				$(c_ctrlPrefix+"shinkiMsg"+i).removeClass("disnon");
			}
			else {
				$(c_ctrlPrefix+"shinkiMsg"+i).addClass("disnon");
			}
		} 
				
	}
	
	function errorClear() {
	var setData = { 
			hlErrorflg		    : "",								
			hlErrorLine1		: "",								
			hlErrorLine2		: "",								
			hlErrorLine3		: "",
			hlErrorLine4		: ""								
		};
		$.rwat.api.setDataToLayout(c_myLayoutId, setData);	
	}
	
	function dataClear() {
    		$(c_ctrlPrefix+"lsiLineNo1").parents("table").find("tr").addClass("disnon");
    		if ($(c_ctrlPrefix+"lbTotalCnt").rwatVal() + 0 == 0)
    		{
    			$(c_ctrlPrefix+"btSetMeishiImport").rwatDisabled(true); 
    			$(c_ctrlPrefix+"btSetKokyaku").rwatDisabled(true);     			    			   	
    		}	
	}
	
} ());