
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Jim Carter>
-- Create date: <9/12/2010>
-- Description:	<Copy TReadings Records to Readings Table for session>
-- =============================================
ALTER PROCEDURE [dbo].[sp_Copy_Session_Records] 
    @session uniqueidentifier
AS
BEGIN
	SET NOCOUNT ON;

	/*
	* Create temp table with readings (only one per site/channel/time)
	* The data from abb recorders has some multiple entries for given site/channel/time
	*/
	select t.sitename, t.chname, t.dtime, avg(t.reading) as reading, session
	into #t
	from treadings t
    where session = @session
	group by sitename, chname, dtime, session

	/*
	* Insert missing readings into 'readings'.
	*/
	insert into readings
	( sitename, chname, dtime, reading )
	select t.sitename, t.chname, t.dtime, t.reading
	from #t t
	left join readings r on 
	( t.sitename = r.sitename ) and ( t.chname = r.chname ) and ( t.dtime = r.dtime )
	where
	( t.session = @session ) and ( r.id is null ) 

    delete treadings
    where session = @session
END
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

