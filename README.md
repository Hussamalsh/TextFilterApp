# TextFilterApp

TextFilterApp is a C# .NET Core console application designed to apply multiple text filters on strings read from a text file. It demonstrates the use of asynchronous programming, dependency injection, and the application of design patterns to create a scalable and maintainable solution.

## Features

- **Read Text File:** Reads input from a specified text file.
- **Apply Filters:** Filters out words based on three criteria:
  - Words with a vowel in the middle.
  - Words shorter than three characters.
  - Words containing the letter 't'.
- **Asynchronous Processing:** Processes text files in chunks asynchronously for improved performance.
- **Modular Design:** Utilizes dependency injection and interface-based design for easy extension and maintenance.

## Getting Started

### Prerequisites

Ensure you have the following installed:
- .NET 8.0 SDK or later

### Installation

1. Clone the repository to your local machine:
   ```sh
   git clone https://github.com/Hussamalsh/TextFilterApp.git
   ```
2. Navigate to the project directory:
   ```sh
   cd TextFilterApp
   ```

### Configuration

1. Modify the `appsettings.json` file to specify the path to the input text file and adjust filter settings if necessary.

### Running the Application

1. Build the application:
   ```sh
   dotnet build
   ```
2. Run the application:
   ```sh
   dotnet run
   ```

## Usage

The application is executed from the command line and does not require any arguments. It reads the configuration from `appsettings.json`, processes the specified text file, and outputs the filtered text to the console.

## Extending the Application

To add a new text filter:
1. Implement the `IFilterStrategy` interface in a new class under the `Filters` directory.
2. Register the new filter in `HostConfiguration.cs` by adding it to the list of filters in the `TextFilter` service configuration.

## Future Improvements

While the TextFilterApp serves its purpose well, there's always room for growth and enhancement. Here are a few areas we're looking to improve in the future:

- **Performance Optimization:** Although the app currently processes files in chunks asynchronously, we aim to further optimize performance, especially for very large files. This could involve more sophisticated asynchronous processing techniques or leveraging parallel computing where appropriate.
- **Expanded File Support:** Currently, the application reads from text files. Expanding this to include other file types, such as PDFs or Word documents, would greatly increase its utility.
- **Adding Logging:** we can add logging to the application to help with debugging and monitoring.
- **Adding Documentation:** We can add more documentation and comments to the code to make it easier for developers to understand and contribute to the project.

## Contributing

Contributions are welcome! Please feel free to submit pull requests with new features, improvements, or bug fixes.

## Author

❤️ Hussam ❤️