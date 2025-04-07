-- Add Temperature column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[charecterstics]') AND name = 'Temperature')
BEGIN
    ALTER TABLE [dbo].[charecterstics] ADD Temperature float NOT NULL DEFAULT 0.0;
END

-- Add Humidity column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[charecterstics]') AND name = 'Humidity')
BEGIN
    ALTER TABLE [dbo].[charecterstics] ADD Humidity float NOT NULL DEFAULT 0.0;
END

-- Add Rainfall column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[charecterstics]') AND name = 'Rainfall')
BEGIN
    ALTER TABLE [dbo].[charecterstics] ADD Rainfall float NOT NULL DEFAULT 0.0;
END

-- Add Timestamp column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[charecterstics]') AND name = 'Timestamp')
BEGIN
    ALTER TABLE [dbo].[charecterstics] ADD Timestamp datetime2 NOT NULL DEFAULT GETUTCDATE();
END 