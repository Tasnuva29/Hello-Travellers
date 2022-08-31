create database "Hello Travellers!";
use "Hello Travellers!";

create table [User] (
	Username VARCHAR(32),
	[Name] VARCHAR(32),
	Email VARCHAR(32),
	PhoneNumber VARCHAR(32),
	Gender VARCHAR(8),
	[Password] VARCHAR(32),
	About VARCHAR(256),
	DisplayPictureName VARCHAR(256),
	[Rank] VARCHAR(128) DEFAULT('USER'),
	CONSTRAINT pk_username PRIMARY KEY(Username),
	CONSTRAINT chk_email CHECK(Email like '_%@%_.%_'),
	CONSTRAINT unique_email UNIQUE(Email)
);

create table [Messages] (
	MessageID INT IDENTITY (1,1),
	Content VARCHAR(256),
	SenderUsername VARCHAR(32),
	ReceiverUsername VARCHAR(32),
	SentTime DATETIME DEFAULT GETDATE(),
	[SeenStatus] VARCHAR(16) DEFAULT ('SENT'),
	CONSTRAINT pk_messageID PRIMARY KEY(MessageID),
	CONSTRAINT fk_senderUsername FOREIGN KEY(SenderUsername) REFERENCES [User](Username),
	CONSTRAINT fk_receiverUsername FOREIGN KEY (ReceiverUsername) REFERENCES [User](Username)
)

create table Subforum (
	ForumID INT IDENTITY(1,1),
	ForumName VARCHAR(32),
	CONSTRAINT pk_forumID PRIMARY KEY(ForumID)
)

create table Post (
	PostID INT IDENTITY(1, 1),
	Content VARCHAR(8000),
	Title VARCHAR(128),
	CreatorUsername VARCHAR(32),
	CreationTime DATETIME DEFAULT GETDATE(),
	ForumID INT,
	CONSTRAINT pk_postID PRIMARY KEY(PostID),
	CONSTRAINT fk_creatorUsername FOREIGN KEY(CreatorUsername) REFERENCES [User](Username),
	CONSTRAINT fk_forumID FOREIGN KEY(ForumID) REFERENCES Subforum(ForumID)
)

create table React (
	ReactStatus INT,
	PostID INT NOT NULL,
	Username VARCHAR(32) NOT NULL,
	CONSTRAINT cpk_react PRIMARY KEY(PostID, Username),
	CONSTRAINT fk_postID FOREIGN KEY(PostID) REFERENCES Post(PostID),
	CONSTRAINT fk_username FOREIGN KEY(Username) REFERENCES [User](Username)
)

create table Reply (
	ReplyID INT IDENTITY (1, 1),
	Content VARCHAR(1024),
	PostID INT,
	CreatorUsername VARCHAR(32),
	CreationTime DATETIME DEFAULT GETDATE(),
	CONSTRAINT pk_replyID PRIMARY KEY(ReplyID),
	CONSTRAINT fk_replyCreatorUsername FOREIGN KEY(CreatorUsername) REFERENCES [User](Username),
	CONSTRAINT fk_replyPostID FOREIGN KEY(PostID) REFERENCES Post(PostID)
)

create table MediaItem (
	MediaID INT IDENTITY (1, 1),
	[Type] VARCHAR(32),
	CreationTime DATETIME DEFAULT GETDATE(),
	UploaderUsername VARCHAR(32),
	PostID INT,
	CONSTRAINT pk_mediaID PRIMARY KEY(MediaID),
	CONSTRAINT fk_miUploaderUsername FOREIGN KEY(UploaderUsername) REFERENCES [User](Username),
	CONSTRAINT fk_miPostID FOREIGN KEY(PostID) REFERENCES Post(PostID)
)

create table Report (
	ReportID INT IDENTITY(1, 1),
	Context VARCHAR(32),
	ContextID VARCHAR(32),
	Reason VARCHAR(256),
	ReporterUsername VARCHAR(32),
	[Status] VARCHAR(32) DEFAULT ('UNRESOLVED'),
	CONSTRAINT pk_reportID PRIMARY KEY(ReportID),
	CONSTRAINT fk_ReporterUsername FOREIGN KEY(ReporterUsername) REFERENCES [User](Username)
)

create table [Notification] (
	NotificationID INT IDENTITY(1,1),
	ForUsername VARCHAR(32),
	HtmlContent VARCHAR(4096),
	CreationTime DATETIME DEFAULT GETDATE(),
	SeenStatus VARCHAR(32) DEFAULT ('SENT'),
	CONSTRAINT pk_NotificationID PRIMARY KEY(NotificationID),
	CONSTRAINT fk_ForUsername FOREIGN KEY(ForUsername) REFERENCES [User](Username)
)

CREATE FUNCTION dbo.concatUsernames(@username1 VARCHAR(32), @username2 VARCHAR(32))
RETURNS VARCHAR(64)
AS
BEGIN
	IF (@username1 > @username2) RETURN CONCAT_WS(' ',@username1, @username2)
	RETURN CONCAT_WS(' ', @username2, @username1)
END

INSERT INTO Subforum(ForumName)
VALUES ('Experiences'),
('Travel Plans'),
('Short Stories')

INSERT INTO [User] VALUES('admin', 'Admin', 'bjornthor@gmail.com', '+8801700000000', 'Male', 'Asd123@', 'Travelling is ...', 'default.jpg', 'ADMIN');



/*Test*/
SELECT Content, defTime.SentTime, defTime.MessageID, defTime.SenderUsername, defTime.ReceiverUsername, defTime.SeenStatus 
FROM Messages AS defTime
INNER JOIN
(SELECT dbo.concatUsernames(SenderUsername, ReceiverUsername) AS Parties, MAX(SentTime) AS SentTime FROM Messages 
WHERE SenderUsername = 'bthor' OR ReceiverUsername = 'bthor' 
GROUP BY dbo.concatUsernames(SenderUsername, ReceiverUsername))
AS maxTimeTable
ON defTime.SentTime = maxTimeTable.SentTime

SELECT * FROM [User]
SELECT * FROM Post
SELECT * FROM Subforum
SELECT * FROM MediaItem
SELECT * FROM Reply
SELECT * FROM React
SELECT * FROM Report
SELECT * FROM Messages
SELECT * FROM [Notification]