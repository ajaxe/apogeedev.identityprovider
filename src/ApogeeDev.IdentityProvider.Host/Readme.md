# ApogeeDev.IdentityProvider.Host

Main hosting project for the Identity provider.

## Notes

Starting from v2.28.0 of MongoDB.Driver the dlls are strong named, if latest _MongoDB.EntityFrameworkCore_ is used it breaks compatibility with reference _for \_OpenIddict.MongoDb_ library. To allow the project to build correctly we
will fix EFCore version to _8.0.3_.
