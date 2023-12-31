﻿using System.Text.Json.Serialization;

namespace SharedLibrary.Dtos;

public class Response<T> where T : class
{
    public T Data { get; private set; }
    public int StatusCode { get; private set; }
    public ErrorDto Error { get; set; }
    [JsonIgnore]
    public bool IsSuccesful { get; set; }
    public static Response<T> Success(T data, int statusCode)
    {
        return new Response<T> { Data = data, StatusCode = statusCode, IsSuccesful = true };
    }
    public static Response<T> Success(int statusCode)
    {
        return new Response<T> { Data = default!, StatusCode = statusCode, IsSuccesful = true };
    }
    public static Response<T> Fail(ErrorDto errorDto, int statusCode)
    {
        return new Response<T> { Error = errorDto, StatusCode = statusCode, IsSuccesful = false };
    }
    public static Response<T> Fail(string errorMessage, int statusCode, bool isShow)
    {
        var errorDto = new ErrorDto(errorMessage, isShow);
        return new Response<T> { Error = errorDto, StatusCode = statusCode, IsSuccesful = false };
    }
}