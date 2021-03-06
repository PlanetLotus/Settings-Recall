<h1>Development</h1>

Here we detail what's left to develop.

<h2>Main Functionality</h2>

<ul>
    <li>Restore Backup</li>
</ul>

<h2>Additional Features</h2>

These are intended to be a part of the program before calling it "done".

<ul>
    <li>Edit Program: Allow name change (just delete current entry and create a new one)</li>
    <li>Don't allow deletion of built-in (IsPermanent == true) programs. Do
    allow deletion of user-added programs.</li>
    <li>Catch all exceptions / handle all errors "client-side", producing a friendly
    message before blowing up, preferably before going out to the db.</li>
</ul>

<h2>Desired Features</h2>

These are non-essential, non-1.0 features.

<ul>
    <li>New Program: Be able to choose a pre-existing program as a "template" for a new program</li>
    <li>Dry run: For both create and restore backup, create a "dry run" feature
    that shows a list of what would be copied. Great safety feature, and would
    help us test the program.</li>
</ul>
