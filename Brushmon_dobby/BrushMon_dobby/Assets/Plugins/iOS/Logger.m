#import "LoggerHelper.h"
// Converts C style string to NSString
#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]
extern void _log(const char* message)
{
	LoggerHelper* helper = [[LoggerHelper alloc]init];
    [helper log:GetStringParam(message)];
}

extern char* _userCountryCode()
{
	LoggerHelper* helper = [[LoggerHelper alloc]init];
    NSString* code = [NSString stringWithFormat:@"%@",[helper userCountryCode]];
    const char *codeChar = [code cStringUsingEncoding:NSUTF8StringEncoding];
    return codeChar ? strdup(codeChar):NULL;
}

extern char* _userLanguageCode()
{
	LoggerHelper* helper = [[LoggerHelper alloc]init];
    NSString* code = [NSString stringWithFormat:@"%@",[helper userLanguageCode]];
    const char *codeChar = [code cStringUsingEncoding:NSUTF8StringEncoding];
    return codeChar ? strdup(codeChar):NULL;
}
