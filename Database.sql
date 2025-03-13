
-- Create Database
CREATE DATABASE TaskManagementDB;
USE TaskManagementDB;

-- Create Users Table
CREATE TABLE Users (
    UserId INT AUTO_INCREMENT PRIMARY KEY,
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    Role ENUM('Admin', 'User', 'Manager') NOT NULL DEFAULT 'User',
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Create Tasks Table
CREATE TABLE Tasks (
    TaskId INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    Description TEXT,
    Status ENUM('Pending', 'In Progress', 'Completed') NOT NULL DEFAULT 'Pending',
    AssignedTo INT,
    CreatedBy INT NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (AssignedTo) REFERENCES Users(UserId) ON DELETE SET NULL,
    FOREIGN KEY (CreatedBy) REFERENCES Users(UserId) ON DELETE CASCADE
);

-- Create Comments Table
CREATE TABLE Comments (
    CommentId INT AUTO_INCREMENT PRIMARY KEY,
    TaskId INT NOT NULL,
    UserId INT NOT NULL,
    Comment TEXT NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (TaskId) REFERENCES Tasks(TaskId) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

-- Create Notifications Table
CREATE TABLE Notifications (
    NotificationId INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    Message TEXT NOT NULL,
    IsRead BOOLEAN DEFAULT FALSE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

-- Insert Sample Users
INSERT INTO Users (FullName, Email, PasswordHash, Role) VALUES
('Admin User', 'admin@example.com', 'hashed_password', 'Admin'),
('Manager User', 'manager@example.com', 'hashed_password', 'Manager'),
('Employee User', 'user@example.com', 'hashed_password', 'User');

-- Insert Sample Tasks
INSERT INTO Tasks (Title, Description, Status, AssignedTo, CreatedBy) VALUES
('Setup Database', 'Create database schema and tables', 'Pending', 2, 1),
('Develop API', 'Implement RESTful API endpoints', 'In Progress', 3, 1),
('Write Documentation', 'Prepare README and API docs', 'Pending', 3, 2);

-- Insert Sample Comments
INSERT INTO Comments (TaskId, UserId, Comment) VALUES
(1, 2, 'Started working on the database.'),
(2, 3, 'API development is in progress.'),
(3, 3, 'Will start documentation soon.');

-- Insert Sample Notifications
INSERT INTO Notifications (UserId, Message, IsRead) VALUES
(2, 'You have been assigned a new task: Setup Database', FALSE),
(3, 'Your task "Develop API" is in progress', FALSE),
(3, 'Your task "Write Documentation" is pending', FALSE);
