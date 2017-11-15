IF EXISTS(SELECT * FROM sysobjects WHERE name='KK_SaveMeishiData')
   DROP PROCEDURE dbo.KK_SaveMeishiData
GO
CREATE PROC KK_SaveMeishiData
@KAISHA_CD  NVARCHAR(15),                                              --��ЃR�[�h
@USER_ID    NVARCHAR(30),                                              --���[�UID
@PGM_CD     NVARCHAR(30),                                              --�v���O�����R�[�h
@FILE_PATH  NVARCHAR(200),                                             --�t�@�C���p�X
@SHUTOKU_DATE NVARCHAR(20),										   	       --�擾��
@TANTOSHA_CD  NVARCHAR(10),											   --�S����
@SESSION_ID	NVARCHAR(50),											   --�Z�b�V����ID
@GET_ROW_FR		INT,
@GET_ROW_TO		INT
AS
BEGIN
    --INT�ϐ����^�[���R�[�h(@RetCd)�������l0�Œ�`�B
    DECLARE @RetCd INT = 0
    --���ݓ�����ϐ�
    DECLARE @NOW DATETIME = GETDATE()
	DECLARE @NOW_TIME DATETIME =  CONVERT(NVARCHAR(8),GETDATE(),108)
	DECLARE @ERR_MSG 	NVARCHAR(20) = '�C��'

	DELETE FROM KK_TBL_MEISHI_WK 
	WHERE KK_TBL_MEISHI_WK.SESSION_ID = @SESSION_ID
	AND KK_TBL_MEISHI_WK.KAISHA_CD = @KAISHA_CD
    
    --�ꎞ�e�[�u���Ƀf�[�^���ꊇ�}������
    BEGIN TRY
      DECLARE @CMD NVARCHAR(1000) 
        SET @CMD =
        'BULK INSERT KK_TBL_MEISHI_WK FROM '''
            + @FILE_PATH +
            ''' WITH (
                FIELDTERMINATOR='','',
                KEEPNULLS
        )'
        EXECUTE(@CMD)
    END TRY
    BEGIN CATCH
      SET @RetCd = 1;
    --�I������
     GOTO END_PROC
    END CATCH
    
	--�S���҈ꊇ�ݒ�̎w���D�悷��
    IF @TANTOSHA_CD IS NOT NULL AND  @TANTOSHA_CD <> '' BEGIN
		 UPDATE KK_TBL_MEISHI_WK
		 SET 
		 KK_TBL_MEISHI_WK.TANTOSHA_CD = @TANTOSHA_CD
		 WHERE  SESSION_ID = @SESSION_ID
		  AND KAISHA_CD = @KAISHA_CD
	END

	--�擾���ꊇ�ݒ�̎w��l��D�悷��
	IF @SHUTOKU_DATE IS NOT NULL AND  @SHUTOKU_DATE <> '' BEGIN
		 UPDATE KK_TBL_MEISHI_WK
		 SET 
		 KK_TBL_MEISHI_WK.SHUTOKU_DATE = @SHUTOKU_DATE
		 WHERE  SESSION_ID = @SESSION_ID
		 AND KAISHA_CD = @KAISHA_CD
	END

	EXEC @RetCd = dbo.KK_CheckMeishiData
					@KAISHA_CD		= @KAISHA_CD			--��ЃR�[�h
					,@USER_ID		= @USER_ID             --���[�UID
					,@PGM_CD		= @PGM_CD              --�v���O�����R�[�h	
					,@SESSION_ID	= @SESSION_ID		   --�Z�b�V����ID					
	IF (@RetCd <> 0)
	BEGIN
		SET @RetCd = 1;			--�G���[

		GOTO END_PROC
	END
	--�擾�������ǉ�
	UPDATE KK_TBL_MEISHI_WK SET
	KK_TBL_MEISHI_WK.SHUTOKU_DATE = FORMAT(CONVERT(DATETIME,CAST(CONVERT(NVARCHAR(100),KK_TBL_MEISHI_WK.SHUTOKU_DATE, 111)+ @NOW_TIME AS DATETIME)),'yyyy/MM/dd HH:mm:ss') 
	WHERE  SESSION_ID = @SESSION_ID
	AND KAISHA_CD = @KAISHA_CD
	AND KK_TBL_MEISHI_WK.SHUTOKU_DATE IS NOT NULL 
	AND ISDATE(KK_TBL_MEISHI_WK.SHUTOKU_DATE) = 1

    --�ϐ����^�[���R�[�h(@RetCd)�𒊏o����B
   		SELECT @RetCd
    --KK_TBL_MEISHI_WK�����L�����Œ��o����
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
		 AND KAISHA_CD = @KAISHA_CD
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

	SELECT     		
    COUNT(*)
	FROM KK_TBL_MEISHI_WK
	WHERE KK_TBL_MEISHI_WK.SESSION_ID = @SESSION_ID
	AND KK_TBL_MEISHI_WK.ERROR_MSG LIKE '%' + @ERR_MSG + '%' COLLATE Japanese_CI_AI
	AND KK_TBL_MEISHI_WK.ERROR_MSG IS NOT NULL
	AND KK_TBL_MEISHI_WK.KAISHA_CD = @KAISHA_CD


    RETURN
    --END_PROC���x���̏���
END_PROC:
    SELECT @RetCd
END
