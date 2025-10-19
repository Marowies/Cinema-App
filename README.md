 CinemaApp.NET Backend API

CinemaApp is a simple C# .NET Web API project that simulates a cinema booking system.  
It allows two types of users — Admin and Regular User to interact with movies, showtimes, and tickets.


 Features
  Admin
- Add new movies.  
- Set movie showtimes (start and end time).  
- Remove movies after their end date automatically.  
- Manage upcoming movies and schedules.

 Regular User
- View available movies and their showtimes.  
- Book movie tickets.  
- Cancel a booked ticket before the movie starts.  
- Tickets automatically expire when the movie’s end time passes.
--------------------------------------------------------------------------
Project Architecture
 Layered structure inside CinemaApp:
 Controllers/ # API controllers (Movies, Tickets, Auth)
 Data/ # Database context (ApplicationDbContext)
 Models/ # Data models (Movie, ShowTime, Ticket, User)
 Migrations/ # EF Core migrations
 appsettings.json # Database connection & JWT configuration
 
 -------------------------------------------------------------------------
Database Design

Tables:
- Users
  - UserId, Username, Password, Role (Admin/User)
- Movies
  - MovieId, Title, Description, Duration, StartDate, EndDate
- ShowTimes
  - ShowTimeId, MovieId, StartTime, EndTime
- Tickets
  - TicketId, UserId, ShowTimeId, BookingDate, Status

 Technologies Used
- C# .NET 8 / ASP.NET Core Web API  
- Entity Framework Core  
- SQL Server (LocalDB)  
- JWT Authentication  
- LINQ for queries  
- Dependency Injection (DI)  
-------------------------------------------------------------------------
Authentication
This project uses JWT (JSON Web Token)for login and role-based authorization:
| Role | Access |
| Admin | Can add/edit/delete movies, manage showtimes |
| User | Can view movies, book and cancel tickets |
When  you log in, you receive a JWT token that must be added to request headers:
Authorization: Bearer <your_token_here>

----------------------------------------------------------------------------
 API Endpoints

 AuthController
| Method | Endpoint | Description |
| POST | api/auth/register| Register a new user |
| POST | api/auth/login| Login and receive JWT token |

 MoviesController
| Method | Endpoint | Description |
| GET | api/movies | Get all movies |
| POST | api/movies | Add a new movie (Admin only) |
| DELETE | api/movies/{id} | Remove a movie (Admin only) |

 ShowTimesController
| Method | Endpoint | Description |
| POST | api/showtimes | Add showtime for a movie (Admin only) |
| GET | api/showtimes/movie/{movieId} | Get showtimes for a specific movie |

 TicketsController
| Method | Endpoint | Description |
| POST | api/tickets/book?userId={userId}&showTimeId={showTimeId} | Book a ticket |
| PUT | api/tickets/cancel/{id} | Cancel a booked ticket |
| GET | api/tickets | Get all tickets |




 
