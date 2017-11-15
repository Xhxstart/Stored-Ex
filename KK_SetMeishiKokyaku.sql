IF EXISTS(SELECT * FROM sysobjects WHERE name='KK_SetMeishiKokyaku')
   DROP PROCEDURE dbo.KK_SetMeishiKokyaku
GO
CREATE PROC KK_SetMeishiKokyaku
@KAISHA_CD  NVARCHAR(15),                                              --��ЃR�[�h
@USER_ID    NVARCHAR(30),                                              --���[�UID
@PGM_CD     NVARCHAR(30),                                              --�v���O�����R�[�h
@SESSION_ID	NVARCHAR(50),											   --�Z�b�V����ID
@KOKYAKUKANRI_NO DECIMAL(10, 0),
@GET_ROW_FR		INT,
@GET_ROW_TO		INT
AS
BEGIN
	DECLARE @NOW DATETIME = GETDATE()
	--�o�^�ڋq���ׂĂɑ΂��āu�ڋq�ꊇ�ݒ�v�őI�������ڋq��ݒ肷��B
	UPDATE KK_TBL_MEISHI_WK SET
	KK_TBL_MEISHI_WK.KOKYAKUKANRI_NO = @KOKYAKUKANRI_NO,
	KK_TBL_MEISHI_WK.UPD_SHAIN_CD = @USER_ID,
	KK_TBL_MEISHI_WK.UPD_PGM_CD = @PGM_CD,
	KK_TBL_MEISHI_WK.UPD_TM = @NOW
	WHERE KK_TBL_MEISHI_WK.SESSION_ID = @SESSION_ID
	AND KK_TBL_MEISHI_WK.KAISHA_CD = @KAISHA_CD

	--INT�ϐ����^�[���R�[�h(@RetCd)�������l0�Œ�`�B
    DECLARE @RetCd INT = 0
    --�ϐ����^�[���R�[�h(@RetCd)�𒊏o����B
   	SELECT @RetCd
    --�����L�����Œ��o����
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
    --END_PROC���x���̏���
END_PROC:
    SELECT @RetCd
END
