# CineMan - A Movie Booking API

Welcome to CineMan - A Movie Booking API! This project is built upon a design solution I built to practice my system design skills, which allows for a modular and scalable design. By separating concerns into distinct domains, I can easily maintain and extend the functionality of my application. The API was built to simulate front of the house activities and making it's operation completely user centric. Thus, don't expect to find any CRUD related operations that are not user oriented.

## Getting Started

To get started with Awesome Project, please follow the instructions below:

1. Clone the repository: `git clone https://github.com/param373r/CineMan.git`
2. Install the required dependencies: `dotnet restore`
3. Configure your appsettings values and secrets.
4. Build and run the application: `dotnet run`
5. Access the application in your browser: `http://localhost:5001`


## Major features

- **Booking CRUD**: I have implemented domain design in such a way to manage user bookings while maintaining a consistent state across other tables in the DB. So any seat booking and cancellation stays in sync with the available seats for a particular showtime for a particular movie on a particular date in that particular theatre.

- **Email Verification**: I have also implemented email verification functionality using SendGrid, allowing me to send booking confirmation emails and enhance the security of my application.

- **Forgot Password functionality**: I understand that passwords can be forgotten. That's why I have implemented a forgot password functionality, which will mail you a reset URL for you to enter your new password, making it easy for you to reset your password securely.

## Technical Details

- **Extensive Querying**: I have implemented extensive querying functionality for movies based on format, genre, and language of availability. This allows users to easily search for movies that meet their specific criteria.
    - Users can also sort the movie results based on various parameters such as release date, rating, or popularity. I have also implemented pagination functionality. Users can navigate through the movie list using pagination controls.

- **SQLite with Entity Framework**: I have integrated SQLite with Entity Framework to provide a lightweight and efficient database solution for my application.

- **Strong Password Requirements**: Security is my top priority. I have implemented strong password requirements to ensure that my data remains safe and protected.

- **Salted Password Hashing**: Using the PBKDF2 technique on SHA512 hashing, I have implemented a robust password hashing mechanism that adds an extra layer of security to my application.

- **User Secrets**: I understand the importance of keeping sensitive information secure. That's why I have integrated User Secrets, allowing me to store confidential data outside of my codebase.

- **Options Pattern**: I have leveraged the power of the Options pattern to seamlessly map appsettings values to classes, providing me with a flexible and customizable experience.

- **Result Pattern and ProblemDetails**: My project follows best practices for API development. I have implemented the Result pattern and ProblemDetails to provide consistent and informative responses to my API consumers.

- **Fluent API for Model Creation**: Creating models should be a breeze. With the fluent API, I can define conversions and serializers effortlessly, saving me time and effort.

- **JWT Service and Refresh Tokens**: Authentication is made simple with the JWT service. I also support refresh tokens, ensuring a seamless and secure user experience. As of now the application doesn't support authorization.

- **Extensive Logging using built in ILogger interface**: Logging is crucial for troubleshooting and monitoring. I have integrated Serilog to provide me with extensive logging capabilities, making it easier to identify and resolve issues.

- **Bogus Library Integration**: Populating my database has never been easier. I have integrated the Bogus library, which automatically populates my database on first bootup, saving me valuable development time.

- **OAS3.1 Standards**: My API is designed following the OpenAPI Specification 3.1 standards. This ensures consistency, interoperability, and ease of integration with other systems.

- **Custom Auth middleware**: I have developed custom auth middleware to cater identity specific needs across all endpoints, allowing me to extend and customize the functionality of my application.

- **Annotated swagger documents**: I have used `Swashbuckle.AspNetCore.Annotations` package to add summary and description for each endpoint, generating a rich OpenAPI document.

- **Added CORS support**: I have also configured support for CORS to prevent unauthorized cross-origin requests.

## Contributing

I welcome contributions from the community! If you have any ideas, bug reports, or feature requests, please open an issue or submit a pull request. Let's make Awesome Project even better together!

## Contact

If you have any questions or need further assistance, please feel free to reach out to me at [X](https://x.com/param373r)

