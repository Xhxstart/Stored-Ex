IF EXISTS(SELECT * FROM sysobjects WHERE name='KK_SetMeishiKokyaku')
   DROP PROCEDURE dbo.KK_SetMeishiKokyaku
GO
CREATE PROC KK_SetMeishiKokyaku
@KAISHA_CD  NVARCHAR(15),                                              --会社コード
@USER_ID    NVARCHAR(30),                                              --ユーザID
@PGM_CD     NVARCHAR(30),                                              --プログラムコード
@SESSION_ID	NVARCHAR(50),											   --セッションID
@KOKYAKUKANRI_NO DECIMAL(10, 0),
@GET_ROW_FR		INT,
@GET_ROW_TO		INT
AS
BEGIN
	DECLARE @NOW DATETIME = GETDATE()
	--登録顧客すべてに対して「顧客一括設定」で選択した顧客を設定する。
	UPDATE KK_TBL_MEISHI_WK SET
	KK_TBL_MEISHI_WK.KOKYAKUKANRI_NO = @KOKYAKUKANRI_NO,
	KK_TBL_MEISHI_WK.UPD_SHAIN_CD = @USER_ID,
	KK_TBL_MEISHI_WK.UPD_PGM_CD = @PGM_CD,
	KK_TBL_MEISHI_WK.UPD_TM = @NOW
	WHERE KK_TBL_MEISHI_WK.SESSION_ID = @SESSION_ID
	AND KK_TBL_MEISHI_WK.KAISHA_CD = @KAISHA_CD

	--INT変数リターンコード(@RetCd)を初期値0で定義。
    DECLARE @RetCd INT = 0
    --変数リターンコード(@RetCd)を抽出する。
   	SELECT @RetCd
    --を下記条件で抽出する
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

    RETURN
    --END_PROCラベルの処理
END_PROC:
    SELECT @RetCd
END
