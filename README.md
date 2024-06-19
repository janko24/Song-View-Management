# Song-View-Management
The Song View Management project simplifies song management and analysis for music industry clients. Users can manage their catalog, track views, and generate analytics. Features include creating, editing, and deleting songs, tracking view counts, and exporting detailed PDF reports summarizing song statistics for comprehensive performance insights.
<h2>Installation Instructions</h2>
    <h3>Song View Management Project:</h3>
    <ol>
        <li>Ensure you have the following prerequisites installed:
            <ul>
                <li>.NET Framework (version 7.0 or above)</li>
                <li>Visual Studio IDE (or any other .NET development environment)</li>
                <li>MySQL Server (or any other relational database management system)</li>
            </ul>
        </li>
        <li>Clone or download the Song View Management project repository from the provided source.</li>
        <li>Open the project solution file in Visual Studio.</li>
        <li>Restore the NuGet packages for the solution.</li>
        <li>Modify the database connection string as well as the API key of Youtube API in the <code>appsettings.json</code> file to point to your MySQL Server instance.</li>
        <li>Open Package Manager Console and run the migration commands to create the database schema:
            <pre><code>Update-Database</code></pre>
        </li>
    </ol>
    <h3>Image Sender Project:</h3>
    <p>Follow the same instructions as for the Song View Management Project, with the following changes:</p>
    <ol>
        <li>Modify the email and folder path settings in the <code>appsettings.json</code> file as per your requirements.</li>
        <li>Modify Client ID and Secret in the <code>GmailServiceHelper.cs</code> class.</li>
    </ol>
    <h3>SongViewUpdateService:</h3>
    <ol>
        <li>Modify the the API key of Youtube API </li>
    </ol>
