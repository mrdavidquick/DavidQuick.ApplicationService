# Log

## Asummptions

-   I can ignore all the compiler warnings present in the initial tembo repo
-   There is no app entry point required, only need tests to work and pass
-   No exceptions occur!
-   Testing can be very **opinionated**, so I've just made sure they pass. Happy to discuss

## Decisions

-   The original test was actually an integration test, so I've only mocked what would be external dependencies
-   All my tests are the equivelent of an intergration test

## Observations

-   In the README.md in mentioned "You do not need to implement IServiceAdministrator for either Administrators". This interface didn't exist (that I could see) so I didn't know what this actually meant?
-   The Error object could be made better to have specific Errors, not one generic with a status and description. Appreicated this was a **shared** class
-   lots of the **Model** classes give compiler warnings, mainly about either making the properties nullable, or providing a default value. With time this would require a discussion with the other consumers of the shared code

## Todo

-   With more time I'd look into better validation of the minimum payment amount, as a currency code is provided for in the **Money** class, but the minimum payment is only specified in GBP 0.99
-   There is some duplication in validators, this could be refactored to remove it
-   My **AdministratorServiceLocator** class is hacky, and not thread safe. I had to force the test classes to not run in parallel. With more time I'd resolve this
-   With more time i'd refactor my **ProductTwoApplicationProcessorStrategy** class as everything happens directly inside the **Process** method.
-   There's no unit tests, I would add these with more time
-   Add more domain events, perhaps
    -   KycSuccess
    -   ApplicationFailed
    -   PaymentCreated
-   Add **metrics** and **observability** via **Prometheus** and **OpenTelemetry**
