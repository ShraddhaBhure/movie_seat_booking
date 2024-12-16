# Movie Seat Booking System

The **Movie Seat Booking System** provides an interactive platform for users to explore movies, book seats, and complete payments, while offering an easy-to-use admin panel for managing movies, bookings, and users. The system integrates **ASP.NET Core MVC**, **SQL Server**, **Stripe**, **Twilio** for real-time notifications, and features various modules for booking, payment, reviews, and more.

![Movie Seat Booking](https://github.com/ShraddhaBhure/movie_seat_booking/blob/master/movie_seat_booking/wwwroot/ProjectImages/ProjectIndex.JPG)

![Movie Seat Booking_T](https://github.com/ShraddhaBhure/movie_seat_booking/blob/master/movie_seat_booking/wwwroot/ProjectImages/ProjectTickets.JPG)
---

## Key Features

### 1. **User Management Module**
- **Profile Management**: Users can create and update their profiles.
- **Login/Logout**: Secure login/logout functionality with session management.
- **Registration**: User registration and email verification.

### 2. **Movie Management Module (Admin Panel)**
- **Add/Edit/Delete Movies**: Admins can manage movie listings, including name, description, cast, genre, and showtimes.
- **Manage Movie Details**: Admins can upload movie posters, trailers, and manage other media content.
- **Schedule Movie Showtimes**: Set up showtimes for movies at different theaters and screens.

### 3. **Movie Discovery Module (User View)**
- **Browse Movies**: Users can browse available movies by categories such as genre, rating, and language.
- **Search Movies**: Users can search movies by name, genre, or cast.
- **Movie Details Page**: View detailed information about a movie, including cast, ratings, reviews, and trailer.

### 4. **Booking and Reservation Module**
- **Select Seats**: After choosing a movie and showtime, users can select their seats for booking.
- **Booking Summary**: A summary page that shows booking details, including selected seats and movie information.
- **Booking Confirmation**: Confirmation email and SMS are sent after a successful booking.

### 5. **Payment Gateway Integration**
- **Payment Options**: Integration with **Stripe** for credit card payments.
- **Payment Confirmation**: Upon successful payment, booking is confirmed, and the user receives a receipt.

### 6. **Order History and Ticket Management**
- **View Past Bookings**: Users can view a history of their bookings.
- **Download Tickets**: Users can download their tickets as PDFs or view them with a QR code.
- **Cancel/Modify Bookings**: Users can modify or cancel bookings (subject to policy).

### 7. **Notifications and Alerts Module**
- **Booking Confirmation**: Automatic email and SMS notifications are sent after booking confirmation.
- **Reminders**: Users receive reminders about upcoming showtimes and bookings.
- **Promotions**: Admins can notify users about upcoming movies, discounts, and promotions.

### 8. **Admin Dashboard and Analytics**
- **Sales & Revenue Reports**: Admins can view statistics and reports about bookings, revenue, and popular movies.
- **User Management**: Admins can manage user accounts.
- **Booking Analytics**: Admins can analyze booking patterns to optimize scheduling.

### 9. **Review and Rating System**
- **Post-Booking Reviews**: Users can leave reviews and ratings for movies they have watched.
- **Admin Moderation**: Admins can moderate reviews and ensure content appropriateness.

### 10. **Security and Authentication**
- **Session Management**: Secure user sessions and password management.
- **Data Encryption**: Encryption of sensitive data like payment details.

---

## Tech Stack

- **Backend**: ASP.NET Core MVC
- **Frontend**: HTML, CSS, JavaScript (jQuery, Bootstrap)
- **Authentication**: ASP.NET Core Identity
- **Database**: SQL Server
- **Payment Gateway**: Stripe
- **SMS/Email Notifications**: Twilio, SMTP
- **File Storage**: Local storage for movie posters, trailers, and other media files
- **Hosting**: Azure (or any other preferred hosting solution)

---

## Installation

### Prerequisites

1. **.NET Core SDK**: Install from [here](https://dotnet.microsoft.com/download).
2. **SQL Server**: Ensure SQL Server is running, or use Azure SQL Database.
3. **Stripe Account**: Create a Stripe account for payment processing.
4. **Twilio Account**: Set up a Twilio account for SMS functionality.
5. **SMTP Server**: Configure your SMTP for email notifications.

### Steps to Run the Project Locally

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/your-repository/movie-seat-booking.git
   cd movie-seat-booking
