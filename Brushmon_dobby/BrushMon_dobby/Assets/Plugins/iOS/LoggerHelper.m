#import "LoggerHelper.h"
@implementation LoggerHelper
- (void) log: (NSString *) message
{
      NSLog(@"Passed message = %@", message);
}
- (NSString *) userCountryCode
{
	NSLocale *currentLocale = [NSLocale currentLocale];
	NSString *countryCode = [currentLocale objectForKey:NSLocaleCountryCode];
	return countryCode;
}
- (NSString *) userLanguageCode
{
	NSLocale *currentLocale = [NSLocale currentLocale];
	NSString *languageCode = [currentLocale objectForKey:NSLocaleLanguageCode];
	return languageCode;
}

@end