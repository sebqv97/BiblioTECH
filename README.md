# BiblioTECH

This is a System designed to administer one or more Libraries, having the features of borrowing an asset that is available, or buying the virtual version of the asset.
When I am referring to an asset, this can be a book, from an old Newspaper to Comics, and also movies, depending of the availability of the Asset in that Library.

## Signing up for a card 

Depending on your location, you will be prompted to the closest one amongst the ones from the System, and after completing the Form with the necessary Data, you will be redirected to that Asset you want to loan or purchase.

As for an administrator, he is the one to add other locations, assets, modify prices and such things. He can also delete them, as well as the Users, and get a statistic about the activity on the platform.

## Different states of assets

An asset, first of all, can't be at all "Available", which will make it impossible to "Place Hold" or "Buy" it. 
If the current asset is being loaned, there is also a Queue that shows up as for how many users opted to loan that before you, and also the current person having it.
If you wanna buy the asset, a virtual copy of it will be sent to the email, after the payment is confirmed.

## Technology Used

First of all, everything is based on the ASP.NET MVC pattern, offering a solid start from where the System began its journey. 
Based on that, another two projects, one containing the services, the User Experience, everything regarding to how every component is working is in the "TechServices" project, linked to the "BiblioTECH"
The other one, named "TechData" contains the Models of all the Entities that the System uses, classes for Users, Assets, Enumerations for States of the Assets and so on.
Moreover, interfaces are declared for them as for services to be able to manipulate the attributes of entities.

