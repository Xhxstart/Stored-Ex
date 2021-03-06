IF EXISTS(SELECT * FROM sysobjects WHERE name='KK_MeishiTorokuCheck')
   DROP PROCEDURE dbo.KK_MeishiTorokuCheck
GO
CREATE PROC KK_MeishiTorokuCheck
@KAISHA_CD  NVARCHAR(15),                                              --会社コード
@USER_ID    NVARCHAR(30),                                              --ユーザID
@PGM_CD     NVARCHAR(30),                                              --プログラムコード
@SESSION_ID	NVARCHAR(50),											   --セッションID
@GET_ROW_FR		INT,
@GET_ROW_TO		INT
AS
BEGIN
	SET NOCOUNT ON;
	--INT変数リターンコード(@RetCd)を初期値0で定義。
	DECLARE @RetCd INT = 1
	DECLARE @ERR_MSG1 	NVARCHAR(20) = '修正'
	DECLARE @ERR_MSG2 	NVARCHAR(20) = '相手先担当者が複数ある'
	DECLARE @NOW DATETIME = GETDATE()
					
	UPDATE KK_TBL_MEISHI_WK
    SET    
	KK_TBL_MEISHI_WK.KOKYAKUKANRI_NO = NULL
	WHERE KK_TBL_MEISHI_WK.KOKYAKUKANRI_NO = 0
	AND  KK_TBL_MEISHI_WK.SESSION_ID = @SESSION_ID
	AND  KK_TBL_MEISHI_WK.KAISHA_CD = @KAISHA_CD
	--CSVファイルを修正する必要があるエラーがある場合
	SELECT	
    	TOP (1) LINE_NO	AS 	LINE_NO1      
    FROM KK_TBL_MEISHI_WK
	WHERE
	  KK_TBL_MEISHI_WK.SESSION_ID = @SESSION_ID
	  AND KK_TBL_MEISHI_WK.ERROR_MSG LIKE '%' + @ERR_MSG1 + '%' COLLATE Japanese_CI_AI
	  AND KK_TBL_MEISHI_WK.KAISHA_CD = @KAISHA_CD
	ORDER BY KK_TBL_MEISHI_WK.LINE_NO

	SELECT	
	TOP (1) LINE_NO	AS 	LINE_NO4 		
     FROM KK_TBL_MEISHI_WK
	 LEFT JOIN KK_TBL_AITESAKI_TANTOSHA AS AITE
		--相手先担当者マスタ．会社コード=引数．会社コード 
          ON AITE.KAISHA_CD = KK_TBL_MEISHI_WK.KAISHA_CD
		 --相手先担当者マスタ．相手担当者コード=一時テーブル．相手担当者コード
          AND AITE.AITESAKI_TANTOSHA_CD = KK_TBL_MEISHI_WK.AYITETANTOSHA_CD 
		  --相手先担当者マスタ．相手担当者コード=一時テーブル．相手担当者コード
          AND AITE.KOKYAKUKANRI_NO = KK_TBL_MEISHI_WK.KOKYAKUKANRI_NO       
		--相手先担当者マスタ．削除フラグ
		  AND AITE.DEL_FLG = 0
    WHERE  KK_TBL_MEISHI_WK.ERROR_MSG IS NULL
		　AND KK_TBL_MEISHI_WK.AYITETANTOSHA_CD IS NOT NULL
		  AND KK_TBL_MEISHI_WK.AYITETANTOSHA_CD <> ''
		  AND KK_TBL_MEISHI_WK.SESSION_ID = @SESSION_ID
		  AND KK_TBL_MEISHI_WK.KAISHA_CD = @KAISHA_CD
		  AND KK_TBL_MEISHI_WK.KOKYAKUKANRI_NO IS NOT NULL
		  AND AITE.KOKYAKUKANRI_NO IS NULL
	ORDER BY KK_TBL_MEISHI_WK.LINE_NO

	--「登録顧客」が未指定のデータがある場合
	SELECT     		
    TOP (1) LINE_NO	AS 	LINE_NO2  		       
    FROM KK_TBL_MEISHI_WK
	WHERE
	  KK_TBL_MEISHI_WK.SESSION_ID = @SESSION_ID
	  AND  KK_TBL_MEISHI_WK.KOKYAKUKANRI_NO IS NULL
	  AND KK_TBL_MEISHI_WK.KAISHA_CD = @KAISHA_CD
	ORDER BY KK_TBL_MEISHI_WK.LINE_NO

	--複数の相手先担当者と一致したデータに、「登録相手先担当者」未指定がある場合
	SELECT     		
    TOP (1) LINE_NO	AS 	LINE_NO3  		       
    FROM KK_TBL_MEISHI_WK
	WHERE
	  KK_TBL_MEISHI_WK.SESSION_ID = @SESSION_ID
	  AND KK_TBL_MEISHI_WK.KOKYAKUKANRI_NO IS NOT NULL
	  AND (KK_TBL_MEISHI_WK.AYITETANTOSHA_CD IS NULL OR KK_TBL_MEISHI_WK.AYITETANTOSHA_CD = '')
	  AND KK_TBL_MEISHI_WK.KAISHA_CD = @KAISHA_CD
	  AND KK_TBL_MEISHI_WK.ERROR_MSG LIKE '%' + @ERR_MSG2 + '%' COLLATE Japanese_CI_AI
	ORDER BY KK_TBL_MEISHI_WK.LINE_NO	

    --KK_TBL_MEISHI_WKを下記条件で抽出する
    SELECT     		
    	LINE_NO,
		KOKYAKUKANRI_NO,
		AYITETANTOSHA_CD,			
		KOKYAKU_NM,
		AYITETANTOSHA_NM,
		ERROR_MSG		
    FROM (
		SELECT LINE_NO,
		AYITETANTOSHA_CD,
		KOKYAKUKANRI_NO,
		AYITETANTOSHA_NM,
		KOKYAKU_NM,
		ERROR_MSG,
		SESSION_ID,
		ROW_NUMBER() OVER ( ORDER BY LINE_NO ) AS row_num
	FROM KK_TBL_MEISHI_WK
	WHERE SESSION_ID = @SESSION_ID
	AND KK_TBL_MEISHI_WK.KAISHA_CD = @KAISHA_CD
	) as t
	WHERE
	  t.row_num BETWEEN @GET_ROW_FR AND @GET_ROW_TO
	  AND t.SESSION_ID = @SESSION_ID	  
	ORDER BY
       LINE_NO ASC
           
	SELECT     		
	COUNT(*)
	FROM KK_TBL_MEISHI_WK
	WHERE KK_TBL_MEISHI_WK.SESSION_ID = @SESSION_ID
		AND KK_TBL_MEISHI_WK.KAISHA_CD = @KAISHA_CD

	END_PROC:
END
