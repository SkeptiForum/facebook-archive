
select      GroupId,
            Type,
            Count(*)
from        ActivityLog
Where       DateCreated > '2015-04-13'
and         DateCreated < '2015-05-14'
group by    GroupId, Type
Order by    GroupId, Type asc

select      *
from        ActivityLog
Where       GroupId = 634810236555246
and         Type = 1
and         DateCreated > '2015-05-13'
order by    DateCreated desc

-- Get all records
select      Type, 
            count(*)
from        ActivityLog
where       Type = 1

select      Distinct
            UserId,
            Min(DateCreated)
from        ActivityLog
group by    UserId
order by    Min(DateCreated) asc

Select      *
from        ActivityLog
--Where       UserId = 10153439969420329
order by    DateCreated desc


select      GroupId,
            Count(*)
from        ActivityLog
Where       Type = 1
Group by    GroupId 
order by    GroupId 

-- Get highest ranking posters
select      Top 100
            UserId, 
            Avg(CAST(LikeCount AS FLOAT)) as AverageLikes,
            Sum(LikeCount)-Count(*) as Coefficient,
            Count(*) as TotalContributions
from        ActivityLog 
group by    UserId
having      Count(*) > 15
order by    Sum(LikeCount)-Count(*) desc

-- Average number of likes per item
select      Top 50
            UserId,
            Avg(Cast(LikeCount AS FLOAT)) as AverageLikes,
            Count(*)
from        ActivityLog
group by    UserId
Having      Count(*) > 5
Order by    Avg(Cast(LikeCount AS FLOAT))/Count(*) desc

select * from ActivityLog where userId = 10152715415192000


-- Get top ten most active commenters
select      Top 10
            UserId, 
            Count(*)
from        ActivityLog 
Where       Type = 2
and         UserId > 0
group by    UserId
order by    Count(*) desc

-- Get top ten most active posters
select      Top 10
            UserId, 
            Count(*)
from        ActivityLog 
Where       Type = 1
and         UserId > 0
group by    UserId
order by    Count(*) desc

-- Get all users that have contributed more than five items
select      UserId,
            Count(*)
From        ActivityLog
group by    UserId
Having      Count(*) > 5

--207969 (07-11 11:07)

-- Get index of posts in [x] group
select      ActivityId,
            DateCreated
from        ActivityLog
Where       GroupId = 280492318756692
and         Type = 1
order by    ActivityId asc

-- Get all items from a given user
select      ActivityID,
            DateCreated
from        ActivityLog
--Where       GroupId = 280492318756692
where       UserId = 10204240976644447
Order by    DateCreated asc

-- Get users that have been active since January
select      userId,
            Count(*),
            Max(DateCreated)
from        ActivityLog
group by    UserId 
having      Max(DateCreated) < '2015-01-09'

-- Get counts for both posts and comments
select      type, count(*)
from        ActivityLog
where       UserId = 10152763178036507
group by    Type 

-- Get Jeremy's counts for Meta and Governance
select      count(*)
from        ActivityLog
where       UserId = 10152763178036507
and (       GroupId = 657796350988948
or          GroupId = 1540900192862656
)
and         Type = 2

